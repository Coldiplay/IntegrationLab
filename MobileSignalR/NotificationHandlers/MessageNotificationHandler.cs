using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using MobileSignalR.Hub;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MobileSignalR.NotificationHandlers.Events;

public class MessageNotificationHandler(
    IConnection connection,
    IHubContext<MobileHub> hubContext,
    IMemoryCache cache,
    ILogger<MessageNotificationHandler> logger)
    : BackgroundService
{
    private IChannel? _channel;
    private const string ExchangeKey = "message.*";
    private const string ExchangeName = "notifications.messages";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _channel = await connection.CreateChannelAsync();

                if (RabbitMqConsumerOptions.DeclareTopology)
                {
                    await _channel.ExchangeDeclareAsync(ExchangeName, type: "topic", durable: true, autoDelete: false);
                    
                    await _channel.QueueDeclareAsync(RabbitMqConsumerOptions.QueueName, durable: true, exclusive: false, autoDelete: false);
                    
                    await _channel.QueueBindAsync(RabbitMqConsumerOptions.QueueName, ExchangeName, ExchangeKey);
                }

                await _channel.BasicQosAsync(0, RabbitMqConsumerOptions.PrefetchCount, global: false);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (sender, ea) =>
                {
                    try
                    {
                        await HandleDeliveryAsync(ea, stoppingToken);
                        await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        // сервис останавливается — не считаем за ошибку
                    }
                    catch (RecoverableException rex)
                    {
                        logger.LogWarning(rex, "Transient error, requeue message. DeliveryTag={Tag}", ea.DeliveryTag);
                        await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to process message. Dead-letter. DeliveryTag={Tag}", ea.DeliveryTag);
                        await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                var consumerTag = await _channel.BasicConsumeAsync(
                    queue: RabbitMqConsumerOptions.QueueName,
                    autoAck: false,
                    consumer: consumer
                );

                logger.LogInformation("RabbitMQ consumer started. Queue={Queue}, Prefetch={Prefetch}", RabbitMqConsumerOptions.QueueName, RabbitMqConsumerOptions.PrefetchCount);

                try
                {
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                }
                finally
                {
                    if (_channel?.IsOpen == true)
                    {
                        try { await _channel.BasicCancelAsync(consumerTag); } catch { /* ignore */ }
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Нормальное завершение
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RabbitMQ consumer loop error. Will retry in 5s.");
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException) { /* stopping */ }
            }
            finally
            {
                try { await _channel?.CloseAsync(); } catch { /* ignore */ }
                try { _channel?.Dispose(); } catch { /* ignore */ }
                _channel = null;
            }
        }
    }

    private async Task HandleDeliveryAsync(BasicDeliverEventArgs ea, CancellationToken ct)
    {
        var contentType = ea.BasicProperties?.ContentType?.ToLowerInvariant();
        if (!string.IsNullOrEmpty(contentType) && contentType != "application/json")
        {
            logger.LogWarning("Unexpected content type: {ContentType}. Dead-lettering.", contentType);
            throw new Exception("Unrecoverable: unexpected content type");
        }

        var bodyBytes = ea.Body.ToArray();
        var body = Encoding.UTF8.GetString(bodyBytes);

        MessageEvent? messageEvent;
        try
        {
            messageEvent = JsonSerializer.Deserialize<MessageEvent>(body, JsonOptions);
        }
        catch (JsonException jex)
        {
            logger.LogWarning(jex, "Invalid JSON. Dead-lettering.");
            throw new Exception("Unrecoverable: invalid JSON", jex);
        }

        if (messageEvent?.Envelope == null || messageEvent.Payload == null)
        {
            logger.LogWarning("Invalid contract (missing envelope/payload). Dead-lettering.");
            throw new Exception("Unrecoverable: invalid contract");
        }

        // Версионирование: поддерживаем v1, остальные — пока пропускаем как есть (можно расширить)
        if (!string.Equals(messageEvent.Envelope.Version, "v1", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogInformation("Unsupported event version {Version}. Still attempting to forward.", messageEvent.Envelope.Version);
        }

        // Идемпотентность по eventId
        var eventId = messageEvent.Envelope.EventId;
        if (!string.IsNullOrWhiteSpace(eventId))
        {
            if (cache.TryGetValue(eventId, out _))
            {
                logger.LogDebug("Duplicate event detected. EventId={EventId} — skipping.", eventId);
                return; // ack наверху
            }

            cache.Set(eventId, true, TimeSpan.FromSeconds(RabbitMqConsumerOptions.IdempotencyTtlSeconds));
        }

        var action = messageEvent.Payload.Action?.ToLowerInvariant() ?? "updated";
        var hubMethod = action switch
        {
            "created" => "message.created",
            "updated" => "message.updated",
            "deleted" => "message.deleted",
            _ => "message.updated"
        };

        var signalrPayload = new
        {
            messageId = messageEvent.Payload.Id,
            chatId = messageEvent.Payload.ChatId,
            userId = messageEvent.Payload.UserId,
            content = messageEvent.Payload.Content,
            updatedAt = messageEvent.Payload.UpdatedAt,
            version = messageEvent.Payload.Version,
            envelope = new
            {
                eventId = messageEvent.Envelope.EventId,
                occurredAt = messageEvent.Envelope.OccurredAt,
                correlationId = messageEvent.Envelope.CorrelationId,
                producer = messageEvent.Envelope.Producer,
                type = messageEvent.Envelope.Type,
                version = messageEvent.Envelope.Version
            }
        };

        try
        {
            await hubContext.Clients
                .Group(messageEvent.Payload.ChatId)
                .SendAsync(hubMethod, signalrPayload, ct);

            logger.LogInformation(
                "SignalR notified: {Method} for message {Id} to group {GroupId}. EventId={EventId}",
                hubMethod, messageEvent.Payload.Id, messageEvent.Payload.ChatId, messageEvent.Envelope.EventId
            );
        }
        catch (Exception ex)
        {
            // Ошибка доставки в SignalR обычно временная — просим брокер переотправить
            throw new RecoverableException("SignalR delivery failed", ex);
        }
    }
}

// Простая обёртка для временных ошибок, чтобы ре-куеить сообщение
public class RecoverableException(string message, Exception? inner = null) : Exception(message, inner);