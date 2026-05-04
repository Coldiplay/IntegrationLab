using System.Collections.Concurrent;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using BaseLibrary.Auth;
using BaseLibrary.Model;
using BaseLibrary.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using MobileSignalR.Auth;
using SignalRSwaggerGen.Attributes;

namespace MobileSignalR.Hub;

[SignalRHub("/hub")]
//[Authorize(Policy = "Authorized")]
public class MobileHub(HttpClient httpApi, JwtTokenHandler checker) : Microsoft.AspNetCore.SignalR.Hub
{
    //private readonly HttpClient httpApi
    // Мобилка <-> SignalR <<-> API (Laravel) <->> Сайт (Laravel)
    
    private readonly ConcurrentDictionary<string, string> _jwtToLaravel = checker.JwtToLaravel;
    public async Task<Response> GetChatMembers(int chatId)
    {
        httpApi.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetLaravelToken());
        return this.ToResponseWithData(await httpApi.GetLaravel<IEnumerable<User>>($"api/chat/{chatId}/members"));
    }

    //TODO: Сделать пагинацию
    public async Task<Response> GetChatMessages(int chatId)
    {
        httpApi.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetLaravelToken());
        return this.ToResponseWithData(await httpApi.GetLaravel<IEnumerable<Message>>($"api/chat/{chatId}/messages"));
    }

    public async Task<Response> GetChats(int userId)
    {
        httpApi.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetLaravelToken());
        return this.ToResponseWithData(await httpApi.GetLaravel<IEnumerable<Chat>>($"api/chat/?userId={userId}"));
    }

    public async Task<Response> GetIncidents(int userId)
    {
        httpApi.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetLaravelToken());
        return this.ToResponseWithData(await httpApi.GetLaravel<IEnumerable<Incident>>($"api/incident/?driverId={userId}"));
    }

    public async Task<Response> GetShippings(int userId)
    {
        httpApi.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetLaravelToken());
        return this.ToResponseWithData(await httpApi.GetLaravel<IEnumerable<Shipping>>($"api/shipping/?driverId={userId}"));
    }

    [AllowAnonymous] 
    public async Task<Response> Authorize(string login, string password)
    {
        var result = await httpApi.PostLaravel<UserAuth>("api/login", new { login, password });
        if (string.IsNullOrEmpty(result?.Token))
            return this.BadResponse("Неверная пара логин-пароль", HttpStatusCode.Unauthorized);
        var token = GenerateToken(DateTime.Now.AddMinutes(30));
        _jwtToLaravel.TryAdd(token, result!.Token);
        result.Token = token;
        return this.ToResponseWithData(result, "Успешная авторизация!");
    }
    
    private static string GenerateToken(DateTime expiry)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var identity = new ClaimsIdentity([
            new Claim("ID", Guid.NewGuid().ToString())
        ]);
        
        var xml = Options.RSA; 
        SecurityKey key = KeyHelper.BuildRsaSigningKey(xml);

        var token = new JwtSecurityToken
        (
            issuer: Options.Issuer,
            audience: Options.Audience,
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
        var token = Context.GetHttpContext()?.Request.Headers.Authorization.ToString().Remove(0, 7);
        return string.IsNullOrEmpty(token) ? _jwtToLaravel!.GetValueOrDefault(token) : null;
    }
}