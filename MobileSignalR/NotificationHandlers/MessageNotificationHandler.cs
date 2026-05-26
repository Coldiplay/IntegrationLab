using System.Text;
using BaseLibrary.Model.Classes;
using Microsoft.AspNetCore.SignalR;
using MobileSignalR.Hub;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MobileSignalR.NotificationHandlers;

public class MessageNotificationHandler(IConnection connection, IHubContext<MobileHub> hubContext, ILogger<MessageNotificationHandler> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(queue: "messages-updates-queue", durable: true, exclusive: false,
            autoDelete: false, cancellationToken: stoppingToken);
        
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, stoppingToken);
        
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (sender, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            try
            {
                var model = JsonConvert.DeserializeObject<Message>(message);

                await hubContext.Clients.Group("ChatGroup" + model.ChatId).SendAsync("ReceiveMessage", model);
                
                // Убираем из очереди
                await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка обработки сообщения из RabbitMQ");
                //Возврат в очередь
                await channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(queue: "messages-updates-queue", autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}