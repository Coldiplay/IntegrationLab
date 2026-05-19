using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BaseLibrary.Auth;
using BaseLibrary.Model.Classes;
using BaseLibrary.Tools;
using IntegrationLab.Tools;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace IntegrationLab.Model;

public class HubHandler
{
    private HubConnection _hub;
    private readonly HubData _hubData;
    private static readonly JsonSerializerSettings Options = new()
    {
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        }
    };

    public HubHandler()
    {
        _hubData = App.Services.GetRequiredService<HubData>();
        //_hub = Initialize(_hubData, connectionString);
        //Start();
    }

    public async Task Start()
    {
        _hub = CreateConnection();
        await StartConnection();
        var user = await Authorize();
        await Initialize(_hub);
        await Load();
    }

    private async Task<bool> StartConnection()
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                await _hub.StartAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            if (_hub.State == HubConnectionState.Connected) return true;
        }

        return _hub.State == HubConnectionState.Connected;
    }

    private HubConnection CreateConnection(string connectionString = GlobalOptions.HUB_URI,
        string? bearerToken = null)
    {
        var connection = new HubConnectionBuilder().WithAutomaticReconnect().WithUrl(connectionString, options =>
        {
            if (!string.IsNullOrEmpty(bearerToken))
            {
                options.Headers.Add("Authorization", "Bearer " + bearerToken);
            }
        }).Build();

        return connection;
    }
    

    public async Task<HubConnection> Initialize(HubConnection connection)
    {
        connection.On("asd", () => { });
        connection.On("ReceiveMessage", async (Message newMessage) =>
        {
            if (_hubData.Chats.TryGetValue(newMessage.Chat, out var tuple))
                //lock (tuple.messages)
                //{
                tuple.messages.Add(newMessage);
            //}
            else
                _hubData.Chats.TryAdd(newMessage.Chat,
                    (
                        [.. await GetChatMembers(newMessage.ChatId)],
                        [.. await GetChatMessages(newMessage.ChatId)]
                    ));
        });

        connection.On("ReceiveShipping", async (Shipping newShipping) =>
        {
            await Task.Run(() =>
            {
                //lock (hubData.Shippings)
                //{
                var oldShipping = _hubData.Shippings.FirstOrDefault(s => s.Id == newShipping.Id);
                if (oldShipping is not null)
                    //lock (oldShipping)
                    //{
                    _hubData.Shippings.InsertInsteadOf(oldShipping, newShipping);
                //}
                else
                    _hubData.Shippings.Add(newShipping);
                //}
            });
        });

        connection.On("UpdateIncident", async (Incident newIncident) =>
        {
            await Task.Run(() =>
            {
                //lock (hubData.Incidents)
                //{
                var oldIncident = _hubData.Incidents.FirstOrDefault(s => s.Id == newIncident.Id);
                if (oldIncident is not null)
                    Helper.ChangeAllProperties(oldIncident, newIncident);
                else
                    //Это вообще как должно случится?
                    throw new Exception("Как так-то");

                //}
            });
        });
        
        return connection;
    }

    public async Task Load()
    {
        var shippings = await GetShippings();
        _hubData.Shippings = [..shippings];
        _hubData.Incidents = [.. await GetIncidents()];
        foreach (var chat in await GetChats())
            _hubData.Chats.TryAdd(chat, ([.. await GetChatMembers(chat.Id)], [.. await GetChatMessages(chat.Id)]));
    }

    public async Task<IEnumerable<User>?> GetChatMembers(ulong chatId) =>
        await GetSomething<IEnumerable<User>>("GetChatMembers", chatId);

    //TODO: Зачем я добавил userId?...
    public async Task<IEnumerable<Chat>?> GetChats() =>
        await GetSomething<IEnumerable<Chat>>("GetChats");

    public async Task<IEnumerable<Message>?> GetChatMessages(ulong chatId) =>
        await GetSomething<IEnumerable<Message>>("GetChatMessages", chatId);

    public async Task<IEnumerable<Incident>?> GetIncidents() =>
        await GetSomething<IEnumerable<Incident>>("GetIncidents");

    public async Task<IEnumerable<Shipping>?> GetShippings() => 
        await GetSomething<IEnumerable<Shipping>>("GetShippings");

    //TODO: Подумать над входом нормальным и убрать default значения
    public async Task<User?> Authorize(string login = "admin", string password = "password")
    {
        var authUser = await GetSomething<UserAuth>("Authorize", login, password);

        _hub = CreateConnection(bearerToken: authUser?.Token);
        await StartConnection();
        
        return authUser?.User;
    }


    private async Task<T?> GetSomething<T>(string methodName, params object?[]? parameters)
    {
        var response = await (parameters?.Length switch
        {
            1 => _hub.InvokeAsync<Response>(methodName, parameters[0]),
            2 => _hub.InvokeAsync<Response>(methodName, parameters[0], parameters[1]),
            3 => _hub.InvokeAsync<Response>(methodName, parameters[0], parameters[1], parameters[2]),
            _ => _hub.InvokeAsync<Response>(methodName)
        });
        var smth = await HandleResponse<T>(response);
        return smth ?? default;
    }


    private static async Task<T?> HandleResponse<T>(Response response)
    {
        if ((int)response.StatusCode < 400)
        {
            Debug.WriteLine("Trying deserialize...");
            try
            {
                return JsonConvert.DeserializeObject<T>(response.Data?.ToString(), Options);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                await MessageBoxManager.GetMessageBoxStandard("Ошибка сериализации данных",  exception.Message)
                    .ShowAsync();
                return default;
            }
        }
        
        //TODO: Поменять потом на response message
        await MessageBoxManager.GetMessageBoxStandard("Ошибка получения данных с сервера",  JsonSerializer.Serialize(response))//response.Message)
            .ShowAsync();
        return default;
    }


    private static ulong CheckUserId(ulong? userId)
    {
        return userId is null or < 1 ? (ulong)App.CurrentDriverId : userId.Value;
    }
}