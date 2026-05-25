using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using BaseLibrary.Tools;
using Microsoft.IdentityModel.Tokens;
using MobileSignalR.Auth;

namespace MobileSignalR.Tools;

public class JwtTokenHandler(ILogger<JwtTokenHandler> logger) : BackgroundService
{
    private readonly ConcurrentDictionary<string, string> _jwtToLaravel = [];
    private readonly SecurityKey _publicKey = KeyHelper.BuildRsaSigningKey(Options.RSA);
    private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
    private readonly HttpClient _httpClient = new() {
        BaseAddress = new Uri(GlobalOptions.API_URI)
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var jwtString in _jwtToLaravel.Keys)
            {
                var jwt = _tokenHandler.ReadJwtToken(jwtString);
                if (new DateTimeOffset(jwt.ValidTo) < DateTimeOffset.UtcNow)
                {
                    logger.LogInformation(
                        "Deleting {jwtStr}, because {dtOffset} < {dtOffsetNow} ({dtValidTo} < {dtOffsetNowSec})",
                        jwtString,
                        new DateTimeOffset(jwt.ValidTo), DateTimeOffset.UtcNow,
                        jwt.ValidTo, DateTimeOffset.UtcNow);
                    _jwtToLaravel.Remove(jwtString, out _);
                }


                _jwtToLaravel.TryGetValue(jwtString, out var token);
                if (string.IsNullOrEmpty(token))
                    _jwtToLaravel.Remove(jwtString,
                        out _); //TODO: Есть ли смысл отправлять запрос с потенциальным null-токеном?

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                if ((await _httpClient.GetAsync("api/Auth/Check", stoppingToken)).StatusCode == HttpStatusCode.OK)
                    continue;

                logger.LogInformation("Deleting token {token} because laravel api said so", token?.Remove(token.Length - 10, 9));
                _jwtToLaravel.Remove(jwtString, out _);
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }


    internal string? GetAuthToken(string token)
    {
        return _jwtToLaravel.TryGetValue(token, out var laravelToken) 
            ? laravelToken 
            : null;
    }

    internal bool AddTokenPair(string token, string laravelToken)
    {
        return _jwtToLaravel.TryAdd(token, laravelToken);
    }
    
    internal string GenerateToken(DateTime expiry, Guid userId)
    {
        var identity = new ClaimsIdentity([
            new Claim("ID", userId.ToString())
        ]);
        
        var token = new JwtSecurityToken
        (
            Options.Issuer,
            Options.Audience,
            identity.Claims,
            DateTime.UtcNow,
            expiry,
            new SigningCredentials(_publicKey, SecurityAlgorithms.RsaSha256,
                SecurityAlgorithms.Sha256Digest)
        );
        var tokenString = _tokenHandler.WriteToken(token);
        return tokenString;
    }
}