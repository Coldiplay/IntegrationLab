using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BaseLibrary.Auth;
using BaseLibrary.Model;
using BaseLibrary.Model.Classes;
using BaseLibrary.Tools;
using IntegrationLab.Tools;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;

namespace IntegrationLab.Model;

public class HubHandler
{
    private HubConnection _hub;
    private readonly HubData _hubData;

    public HubHandler(string connectionString = GlobalOptions.HUB_URI)
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
        _hubData.Shippings.AddRange(await GetShippings());
        _hubData.Incidents.AddRange(await GetIncidents());
        foreach (var chat in await GetChats())
            _hubData.Chats.TryAdd(chat, ([.. await GetChatMembers(chat.Id)], [.. await GetChatMessages(chat.Id)]));
    }

    public async Task<IEnumerable<User>?> GetChatMembers(ulong chatId)
    {
        var response = await SimpleGet("GetChatMembers", chatId);
        return await HandleResponse<IEnumerable<User>>(response);
    }

    //TODO: Зачем я добавил userId?...
    public async Task<IEnumerable<Chat>?> GetChats(ulong? userId = null)
    {
        var response = await SimpleGet("GetChats", CheckUserId(userId));
        return await HandleResponse<IEnumerable<Chat>>(response);
    }

    public async Task<IEnumerable<Message>?> GetChatMessages(ulong chatId)
    {
        var response = await SimpleGet("GetChatMessages", chatId);
        return await HandleResponse<IEnumerable<Message>>(response);
    }

    public async Task<IEnumerable<Incident>?> GetIncidents(ulong? userId = null)
    {
        var response = await SimpleGet("GetIncidents", CheckUserId(userId));
        return await HandleResponse<IEnumerable<Incident>>(response);
    }

    public async Task<IEnumerable<Shipping>?> GetShippings(ulong? userId = null)
    {
        var response = await SimpleGet("GetShippings", CheckUserId(userId));
        return await HandleResponse<IEnumerable<Shipping>>(response);
    }

    //TODO: Подумать над входом нормальным и убрать default значения
    public async Task<User?> Authorize(string login = "admin", string password = "password")
    {
        var response = await SimpleGet("Authorize", login, password);
        var authUser = await HandleResponse<UserAuth>(response);

        _hub = CreateConnection(bearerToken: authUser?.Token);
        await StartConnection();
        
        return authUser?.User;
    }


    private async Task<Response> SimpleGet(string methodName)
    {
        return await _hub.InvokeAsync<Response>(methodName);
    }

    private async Task<Response> SimpleGet(string methodName, object parameter)
    {
        return await _hub.InvokeAsync<Response>(methodName, parameter);
    }

    private async Task<Response> SimpleGet(string methodName, object parameter, object parameter2)
    {
        return await _hub.InvokeAsync<Response>(methodName, parameter, parameter2);
    }


    private static async Task<T?> HandleResponse<T>(Response response)
    {
        if ((int)response.StatusCode < 400)
            try
            {
                return (T)(response.Data ?? throw new NullReferenceException());
            }
            catch (Exception e)
            {
                return ((JsonElement)response.Data).Deserialize<T>(GlobalOptions.JsonSerializerOptions);
                
                
                //TODO: Создать обработку ошибки преобразования данных
                if (response.DataTypeName is not null && response.DataTypeName.Equals("array"))
                {
                    //TODO: Придумать как возвращать коллекцию в случае array
                    //return (IEnumerable<T>)(response.Data);
                }

                Console.WriteLine(e);
                throw;
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