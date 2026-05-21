using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BaseLibrary.Auth;
using BaseLibrary.Model.Classes;
using BaseLibrary.Tools;
using IntegrationLab.Tools;
using Microsoft.AspNetCore.SignalR.Client;
using MsBox.Avalonia;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace IntegrationLab.Model;

public class HubHandler
{
    private HubConnection _hub;
    private readonly HubData _hubData;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerSettings Options = new() {
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        }
    };

    public HubHandler(HubData hubData, HttpClient httpClient)
    {
        _hubData = hubData;
        _httpClient = httpClient;
    }

    public async Task Start()
    {
        var userAuth = Authorize();
        await StartConnection();
        await Initialize(_hub);
        await Load();
    }

    private async Task<bool> StartConnection()
    {
        if (_hub.State == HubConnectionState.Connected) return true;
        
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

    private HubConnection CreateConnection(string bearerToken, string connectionString = GlobalOptions.HUB_URI + "/hub")
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
    

    private async Task<HubConnection> Initialize(HubConnection connection)
    {
        connection.On("ReceiveMessage", async (Message newMessage) =>
        {
            if (_hubData.Chats.TryGetValue(newMessage.Chat, out var tuple))
                tuple.messages.Add(newMessage);
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
                var oldShipping = _hubData.Shippings.FirstOrDefault(s => s.Id == newShipping.Id);
                if (oldShipping is not null)
                    _hubData.Shippings.InsertInsteadOf(oldShipping, newShipping);
                else
                    _hubData.Shippings.Add(newShipping);
            });
        });

        connection.On("ReceiveIncident", async (Incident newIncident) =>
        {
            await Task.Run(() =>
            {
                var oldIncident = _hubData.Incidents.FirstOrDefault(s => s.Id == newIncident.Id);
                if (oldIncident is not null)
                    Helper.ChangeAllProperties(oldIncident, newIncident);
                else
                    //Это вообще как должно случится?
                    throw new Exception("Как так-то");
            });
        });
        
        return connection;
    }

    public async Task Load()
    {
        await AwaitForConnection();
        var shippings = await GetShippings();
        _hubData.Shippings = [..shippings];
        _hubData.Incidents = [.. await GetIncidents()];
        foreach (var chat in await GetChats())
            _hubData.Chats.TryAdd(chat, ([.. await GetChatMembers(chat.Id)], [.. await GetChatMessages(chat.Id)]));
    }

    public async Task<IEnumerable<User>?> GetChatMembers(ulong chatId)
    {
        var chatMembers = await GetSomething<IEnumerable<User>>("GetChatMembers", chatId);
        var enumerable = chatMembers?.ToArray();
        
        if (enumerable is not null)
        {
            var chat = _hubData.Chats.FirstOrDefault(c => c.Key.Id == chatId).Value;
            chat.members = [.. enumerable];
        }

        return enumerable;
    }
    public async Task<IEnumerable<Chat>?> GetChats() =>
        await GetSomething<IEnumerable<Chat>>("GetChats");
    public async Task<IEnumerable<Message>?> GetChatMessages(ulong chatId)
    {
        var messages = (await GetSomething<IEnumerable<Message>>("GetChatMessages", chatId))?.ToArray();
        if (messages is not null)
        {
             var chatInfo = _hubData.Chats.FirstOrDefault(c => c.Key.Id == chatId).Value;
             chatInfo.messages = [.. messages];
        }

        return messages;
    }
    public async Task<IEnumerable<Incident>?> GetIncidents()
    {
        var incidents = (await GetSomething<IEnumerable<Incident>>("GetIncidents"))?.ToArray();
        if (incidents is not null)
        {
            _hubData.Incidents = [.. incidents];
        }

        return incidents;
    }
    public async Task<IEnumerable<Shipping>?> GetShippings()
    {
        var shippings = (await GetSomething<IEnumerable<Shipping>>("GetShippings"))?.ToArray();
        if (shippings is not null)
        {
            _hubData.Shippings = [.. shippings];
        }
        
        return shippings;
    }

    
    public async Task<Incident?> CreateIncident(Incident? incident)
    {
        incident = await GetSomething<Incident>("CreateIncident", incident);
        if (incident is not null)
        {
            _hubData.Incidents.Add(incident);
        }

        return incident;
    }
    public async Task<User?> AddChatMember(Chat? chat, User? user)
    {
        user = await GetSomething<User>("AddChatMember", chat, user);
        if (user is not null && _hubData.Chats.TryGetValue(chat!, out var chatInfo))
        {
            chatInfo.members.Add(user);
        }

        return user;
    }
    public async Task<Chat?> CreateChat(Chat? chat)
    {
        chat = await GetSomething<Chat>("CreateChat", chat);
        if (chat is not null)
        {
            _hubData.Chats.TryAdd(chat, ([App.CurrentDriver.User], []));
        }

        return chat;
    }
    public async Task<Message?> SendMessage(Message? message)
    {
        message = await GetSomething<Message>("SendMessage", message);
        
        if (message is null) return message;
        
        var chatInfo = _hubData.Chats.FirstOrDefault(c => c.Key.Id == message.ChatId).Value;
        var oldMessage = chatInfo.messages.FirstOrDefault(m => m.Id == message.Id);
        
        if (oldMessage is null)
            chatInfo.messages.Add(message);
        else
            chatInfo.messages.InsertInsteadOf(oldMessage, message);

        return message;
    }


    public async Task<DriversShift?> StartShift()
    {
        var shift = await GetSomething<DriversShift>("StartShift");
        if (shift is not null)
        {
            _hubData.Shifts.Add(shift);
        }
        return shift;
    }
    public async Task<DriversShift?> EndShift(DriversShift? shift)
    {
        var newShift = await GetSomething<DriversShift>("EndShift", shift);
        if (newShift is not null)
        {
            _hubData.Shifts.InsertInsteadOf(shift!, newShift);
        }
        return newShift;
    }

    public async Task<ShiftBreak?> StartBreak(DriversShift? shift)
    {
        var shiftBreak =  await GetSomething<ShiftBreak>("StartBreak", shift);
        if (shiftBreak is not null)
        {
            shift!.ShiftBreaks.Add(shiftBreak);
        }
        return shiftBreak;
    }
    public async Task<ShiftBreak?> EndBreak(ShiftBreak? shiftBreak)
    {
        var newBreak = await GetSomething<ShiftBreak>("EndBreak", shiftBreak);
        if (newBreak is not null)
        {
            Helper.ChangeAllProperties(shiftBreak!, newBreak);
        }
        return newBreak;
    }
    

    //TODO: Подумать над входом нормальным и убрать default значения
    public async Task<UserAuth?> Authorize(string login = "admin", string password = "password")
    {
        var response = await _httpClient.GetFromJsonAsync<Response>($"api/Auth/Authorize?login={login}&password={password}");

        if (response?.StatusCode != System.Net.HttpStatusCode.OK)
        {
            //TODO: Поменять потом на response message
            await MessageBoxManager.GetMessageBoxStandard("Ошибка авторизации",  JsonSerializer.Serialize(response))//response.Message)
                .ShowAsync();
            return null;
        }
        var authUser = await HandleResponse<UserAuth>(response);

        _hub = CreateConnection(bearerToken: authUser!.Token);
        await StartConnection();
        
        return authUser;
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


    private async Task AwaitForConnection()
    {
        while (_hub?.State != HubConnectionState.Connected)
        {
            await Task.Delay(250);
        }
    }
}