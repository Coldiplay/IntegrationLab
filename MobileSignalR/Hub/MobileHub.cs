using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using BaseLibrary.Auth;
using BaseLibrary.Db;
using BaseLibrary.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using MobileSignalR.Auth;
using SignalRSwaggerGen.Attributes;

namespace MobileSignalR.Hub;

[SignalRHub("/hub")]
[Authorize(Policy = "Authorized")]
public class MobileHub(IntegrationDbContext db, HttpClient httpApi) : Microsoft.AspNetCore.SignalR.Hub
{
    private ConcurrentDictionary<string, string> _jwtToLaravel = [];
    public async Task<Response> GetChatMembers()
    {
        if (GetLaravelToken() is not { } result) return this.BadResponse("Unauthorized", HttpStatusCode.Unauthorized);
        
        httpApi.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result);
        return this.ToResponseWithData(await httpApi.GetLaravel<IEnumerable<User>>($"api/Users/GetChatMembers"));

        // Мобилка <-> SignalR <<-> API (Laravel) <->> Сайт (Laravel)
        // Как авторизовывать мобилки?

    }

    [AllowAnonymous]
    public async Task<Response> Authorize(string login, string passwordHash)
    {
        var result = await httpApi.PostLaravel<UserAuth>("api/Login", new { login, passwordHash });
        if (string.IsNullOrEmpty(result?.Token))
            _jwtToLaravel.TryAdd(result!.Token, GenerateToken(DateTime.Now.AddMinutes(30)));
        return this.ToResponseWithData<object>();
    }
    
    private static string GenerateToken(DateTime expiry)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var identity = new ClaimsIdentity([
            new Claim(ClaimTypes.Name, "...")
            // ... other claims
        ]);
        
        //private key?
        const string xml = "<RSAKeyValue> load...from..local...files...</RSAKeyValue>";
        SecurityKey key = KeyHelper.BuildRsaSigningKey(xml);

        var token = new JwtSecurityToken
        (
            issuer: JwtOptions.Issuer,
            audience: JwtOptions.Audience,
            claims: identity.Claims,
            notBefore: DateTime.UtcNow,
            expires: expiry,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.RsaSha256, SecurityAlgorithms.Sha256Digest)
        );
        var tokenString = tokenHandler.WriteToken(token);
        return tokenString;
    }

    private string? GetLaravelToken()
    {
        return _jwtToLaravel.GetValueOrDefault(Context.GetHttpContext().Request.Headers.Authorization);
    }
}