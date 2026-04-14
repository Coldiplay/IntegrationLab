using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;

namespace MobileSignalR.Auth;

public class JwtTokenHandler : BackgroundService
{
    internal readonly ConcurrentDictionary<string, string> JwtToLaravel = [];
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var jwtString in JwtToLaravel.Keys)
            {
                var jwt = tokenHandler.ReadJwtToken(jwtString);
                if (jwt.ValidTo < DateTime.UtcNow) JwtToLaravel.Remove(jwtString, out _);
            }
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}