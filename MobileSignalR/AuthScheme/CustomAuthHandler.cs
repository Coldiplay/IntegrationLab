using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace MobileSignalR.AuthScheme;

public class CustomAuthHandler(
    IOptionsMonitor<CustomAuthOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    HttpClient client)
    : AuthenticationHandler<CustomAuthOptions>(options, logger, encoder)
{
    private readonly HttpClient _httpClient = client;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var token = Context.User.Claims.FirstOrDefault(c => c.Type.Equals("token_value"))?.Value;
        if (string.IsNullOrWhiteSpace(token))
        {
            return AuthenticateResult.Fail("Неверный токен");
        }
        
        var response = await _httpClient.PostAsJsonAsync($"api/Login", token);
        
        var claims = new[] { new Claim(ClaimTypes.Name, token) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        
        //TODO: Доделать аутентификацию
        return response.StatusCode == HttpStatusCode.OK 
            ? AuthenticateResult.Success(ticket) 
            :  AuthenticateResult.Fail("Неверный токен");
        
        
        return AuthenticateResult.Fail("Неверный токен");
        //throw new NotImplementedException();
    }
}

public class CustomAuthOptions : AuthenticationSchemeOptions
{
}