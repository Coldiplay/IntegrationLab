using BaseLibrary.Auth;
using BaseLibrary.Db;
using BaseLibrary.Model;
using Microsoft.AspNetCore.Authorization;
using SignalRSwaggerGen.Attributes;

namespace MobileSignalR.Hub;

[SignalRHub]
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
}