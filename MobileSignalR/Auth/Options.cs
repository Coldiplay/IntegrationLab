using MobileSignalR.Hub;

namespace MobileSignalR.Auth;

internal static class Options
{
    static Options()
    {
        var data = Environment.GetEnvironmentVariables();
        if (!data.TryGetValue("MOBILE_SIGNALR_ISSUER", out Issuer))
            Issuer = "Issuer";
        if (!data.TryGetValue("MOBILE_SIGNALR_AUDIENCE", out Audience))
            Audience = "Audience";

        if (data.TryGetValue("MOBILE_SIGNALR_SECRET", out RSA)) return;
        
        try
        {
            RSA = File.ReadAllText("private.key");
        }
        catch (Exception e)
        {
            RSA = System.Security.Cryptography.RSA.Create(2048).ToXmlString(true);
        }
    }

    internal static readonly string RSA;
    internal static readonly string Issuer;
    internal static readonly string Audience;
    
    internal static readonly string RabbitMQHostName = "gerbil-01.rmq.cloudamqp.com";
    internal static readonly string RabbitMQUserName = "jqusbezj";
    internal static readonly string RabbitMQPassword = "Qso5el71BSq39Kc3L8uAv5HikHHMNLHy";
    internal static readonly string RabbitMQVirtualHost = "jqusbezj";
}