using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MobileSignalR.Notifications.Events;

public static class RabbitMqConsumerOptions
{
    public static JsonSerializerSettings JsonSettings { get; set; } = new()
    {
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        }
    };
    
    public static string HostName { get; set; } = "gerbil-01.rmq.cloudamqp.com";
    public static string VirtualHost { get; set; } = "jqusbezj";
    public static string UserName { get; set; } = "jqusbezj";
    public static string Password { get; set; } = "Qso5el71BSq39Kc3L8uAv5HikHHMNLHy";

    // Очередь, из которой читаем SignalR-нотификации
    public static string QueueName { get; set; } = "notifications.signalr";

    // Можно авто-объявить топологию (необязательно, если делаете это на стороне продьюсера/инфра)
    public static bool DeclareTopology { get; set; } = true;

    // Prefetch для back-pressure
    public static ushort PrefetchCount { get; set; } = 50;

    // TTL для кэша идемпотентности (по eventId)
    public static int IdempotencyTtlSeconds { get; set; } = 600;

    public static string[] ExchangeNames { get; set; } = ["notifications.messages"];

    public static string[] Patterns { get; set; } = ["*.*"];
}