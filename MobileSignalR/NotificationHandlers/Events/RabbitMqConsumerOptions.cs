namespace MobileSignalR.NotificationHandlers.Events;

public static class RabbitMqConsumerOptions
{
    public static string HostName { get; set; } = "localhost";
    public static int Port { get; set; } = 5672;
    public static string VirtualHost { get; set; } = "/";
    public static string UserName { get; set; } = "guest";
    public static string Password { get; set; } = "guest";

    // Очередь, из которой читаем SignalR-нотификации
    public static string QueueName { get; set; } = "notifications.signalr";

    // Можно авто-объявить топологию (необязательно, если делаете это на стороне продьюсера/инфра)
    public static bool DeclareTopology { get; set; } = true;

    // Prefetch для back-pressure
    public static ushort PrefetchCount { get; set; } = 50;

    // TTL для кэша идемпотентности (по eventId)
    public static int IdempotencyTtlSeconds { get; set; } = 600;
}