using BaseLibrary.Db;
using BaseLibrary.Model;
using BaseLibrary.Tools;
using Microsoft.AspNetCore.Authorization;

namespace MobileSignalR.Hub;

[Authorize]
public class MobileHub(IntegrationDbContext db, HttpClient httpApi) : Microsoft.AspNetCore.SignalR.Hub
{
    public async Task<IEnumerable<User>> GetChatMembers()
    {
        return LaravelParser.ParseResponse<IEnumerable<User>>((await (await httpApi.GetAsync($"api/Users/GetChatMembers")).Content
            .ReadAsStringAsync()));
        return await httpApi.GetFromJsonAsync<IEnumerable<User>>("api/Users/GetChatMembers");
        //if (Clients.Caller.)

        // Мобилка <-> SignalR <<-> API (Laravel) <->> Сайт (Laravel)
        // Как авторизовывать мобилки?

    }


    public async Task<string> Authorize(string login, string passwordHash)
    {
        return "";
    }
}