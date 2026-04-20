using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using BaseLibrary.Tools;

namespace MobileSignalR.Auth;

public class JwtTokenHandler : BackgroundService
{
    internal readonly ConcurrentDictionary<string, string> JwtToLaravel = [];
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri(GlobalOptions.API_URI)
    };
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var jwtString in JwtToLaravel.Keys)
            {
                var jwt = tokenHandler.ReadJwtToken(jwtString);
                if (jwt.ValidTo < DateTime.UtcNow) JwtToLaravel.Remove(jwtString, out _);
                
                JwtToLaravel.TryGetValue(jwtString, out var token);
                if (string.IsNullOrEmpty(token)) JwtToLaravel.Remove(jwtString, out _); //TODO: Есть ли смысл отправлять запрос с потенциальным null-токеном?
                
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                if ((await _httpClient.GetAsync("api/Auth/Check", stoppingToken)).StatusCode == HttpStatusCode.OK) 
                    continue;
                
                JwtToLaravel.Remove(jwtString, out _);
            }
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}