using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

namespace MobileSignalR.Auth;

public class AuthRequirement(HttpClient authClient) : AuthorizationHandler<AuthRequirement>, IAuthorizationRequirement
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        AuthRequirement requirement)
    {
        Debug.WriteLine("Начата проверка");
        var token = context.User.Claims.FirstOrDefault(c => c.Type == "token_value");
        if (token is null || string.IsNullOrWhiteSpace(token.Value))
        {
            context.Fail();
            return;
        }

        authClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
        var response = await authClient.GetAsync("api/Auth/Check");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }
}