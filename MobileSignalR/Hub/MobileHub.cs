using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using BaseLibrary.Auth;
using BaseLibrary.Db;
using BaseLibrary.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using MobileSignalR.Auth;
using SignalRSwaggerGen.Attributes;

namespace MobileSignalR.Hub;

[SignalRHub("/hub")]
[Authorize(Policy = "Authorized")]
public class MobileHub(IntegrationDbContext db, HttpClient httpApi) : Microsoft.AspNetCore.SignalR.Hub
{
    public async Task<Response> GetChatMembers()
    {
        //var token = Context.User!.Claims.First(c => c.Type.Equals("token_value")).Value;
        return this.ToResponseWithData(await httpApi.GetLaravel<IEnumerable<User>>($"api/Users/GetChatMembers"));
        //return await httpApi.GetFromJsonAsync<IEnumerable<User>>("api/Users/GetChatMembers");
        //if (Clients.Caller.)

        // Мобилка <-> SignalR <<-> API (Laravel) <->> Сайт (Laravel)
        // Как авторизовывать мобилки?

    }

    [AllowAnonymous]
    public async Task<Response> Authorize(string login, string passwordHash)
    {
        return this.ToResponseWithData(await httpApi.PostLaravel<UserAuth>("api/Login", new {login, passwordHash}));
    }
    
    private string GenerateToken(DateTime expiry)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var identity = new ClaimsIdentity([
            new Claim(ClaimTypes.Name, "...")
            // ... other claims
        ]);
        
        const string xml = "<RSAKeyValue> load...from..local...files...</RSAKeyValue>";
        SecurityKey key = KeyHelper.BuildRsaSigningKey(xml); 

        var token = new JwtSecurityToken
        (
            issuer: JwtOptions.Issuer,
            audience: JwtOptions.Audience,
            claims: identity.Claims,
            notBefore: DateTime.UtcNow,
            expires: expiry,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest)
        );
        var tokenString = tokenHandler.WriteToken(token);
        return tokenString;
    }
}