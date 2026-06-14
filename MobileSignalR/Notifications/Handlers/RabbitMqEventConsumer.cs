using System.Text;
using Microsoft.Extensions.Caching.Memory;
using MobileSignalR.Notifications.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MobileSignalR.Notifications.Handlers;

public class RabbitMqEventConsumer(
    IConnection connection,
    EventDispatcher dispatcher,
    IMemoryCache cache,
    ILogger<RabbitMqEventConsumer> logger)
    : BackgroundService
{
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueName = RabbitMqConsumerOptions.QueueName;
        var exchangeNames = RabbitMqConsumerOptions.ExchangeNames; // список exchange: ["notifications.messages", "notifications.chatmembers"]
        var routingPatterns = RabbitMqConsumerOptions.Patterns; // ["*.*"]
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _channel = await connection.CreateChannelAsync();

                // Декларируем exchange и queue, привязываем несколько exchange с одним routing key
                foreach (var exchange in exchangeNames)
                {
                    await _channel.ExchangeDeclareAsync(exchange, type: "topic", durable: true, autoDelete: false);
                }

                await _channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);
                foreach (var exchange in exchangeNames)
                {
                    foreach (var pattern in routingPatterns)
                    {
                        await _channel.QueueBindAsync(queueName, exchange, pattern);
                    }
                }

                await _channel.BasicQosAsync(0, RabbitMqConsumerOptions.PrefetchCount, global: false);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (sender, ea) =>
                {
                    try
                    {
                        await ProcessMessage(ea, stoppingToken);
                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                    }
                    catch (RecoverableException rex)
                    {
                        logger.LogWarning(rex, "Recoverable error, requeue. DeliveryTag={Tag}", ea.DeliveryTag);
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Unrecoverable error, dead-letter. DeliveryTag={Tag}", ea.DeliveryTag);
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
                    }
                };

                var consumerTag =
                    await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
                logger.LogInformation("RabbitMQ consumer started on queue {QueueName}", queueName);

                await Task.Delay(Timeout.Infinite, stoppingToken);
                
                if (_channel?.IsOpen == true) {
                    try { await _channel.BasicCancelAsync(consumerTag); } catch { /* ignore */ }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Consumer loop error, retrying in 5s");
                await Task.Delay(5000, stoppingToken);
            }
            finally
            {
                if (_channel?.IsOpen == true) await _channel.CloseAsync();
                _channel?.Dispose();
                _channel = null;
            }
        }
    }
    private async Task ProcessMessage(BasicDeliverEventArgs ea, CancellationToken ct)
    {
        // Проверка content-type и десериализация envelope
        var contentType = ea.BasicProperties?.ContentType?.ToLowerInvariant();
        if (contentType is not (null or "application/json"))
            throw new Exception("Unrecoverable: invalid content type");

        var body = Encoding.UTF8.GetString(ea.Body.ToArray());
        var doc = JsonConvert.DeserializeObject<JToken>(body)!;
        var envelopeRaw = doc["envelope"]!.ToString();
        var payload = doc["payload"]!.ToString();

        var envelope = JsonConvert.DeserializeObject<EventEnvelope>(envelopeRaw, RabbitMqConsumerOptions.JsonSettings);

        if (envelope == null) throw new Exception("Unrecoverable: null envelope");

        // Идемпотентность
        if (!string.IsNullOrWhiteSpace(envelope.EventId))
        {
            if (cache.TryGetValue(envelope.EventId, out _))
            {
                logger.LogDebug("Duplicate event {EventId} ignored", envelope.EventId);
                return;
            }
            cache.Set(envelope.EventId, true, TimeSpan.FromSeconds(RabbitMqConsumerOptions.IdempotencyTtlSeconds));
        }

        await dispatcher.DispatchAsync(envelope, payload, ct);
    }
}