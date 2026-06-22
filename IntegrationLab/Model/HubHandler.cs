using System;
using System.Collections.Generic;
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
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace IntegrationLab.Model;

public class HubHandler(HubData hubData, HttpClient httpClient) : IHubHandler
{
    private HubConnection _hub;

    public async Task Start()
    {
        var userAuth = Authorize();
        await StartConnection();
        Initialize(_hub);
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

    private static HubConnection CreateConnection(string bearerToken, string connectionString = GlobalOptions.HUB_URI + "/hub")
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
    

    private HubConnection Initialize(HubConnection connection)
    {
        connection.On("ReceiveMessage", async (Message newMessage) => {
            if (hubData.Chats.TryGetValue(newMessage.Chat, out var tuple))
                tuple.messages.Add(newMessage);
            else if (newMessage.ChatId != 0)
            {
                var messages = hubData.Chats.FirstOrDefault(c => c.Key.Id == newMessage.ChatId).Value.messages;
                messages?.Add(newMessage);
            }
            else
                hubData.Chats.TryAdd(newMessage.Chat,
                    (
                        [.. await GetChatMembers(newMessage.ChatId)],
                        [.. await GetChatMessages(newMessage.ChatId)]
                    ));
        });
        connection.On("RemoveMessage", (ulong messageId) => {
            var messages = hubData.Chats.FirstOrDefault(c => c.Key.Id == messageId).Value.messages;
            messages?.Remove(messages.FirstOrDefault(m => m.Id == messageId)!);
        });

        connection.On("ReceiveShipping", async (Shipping newShipping) => {
            await Task.Run(() =>
            {
                var oldShipping = hubData.Shippings.FirstOrDefault(s => s.Id == newShipping.Id);
                if (oldShipping is not null)
                    hubData.Shippings.InsertInsteadOf(oldShipping, newShipping);
                else
                    hubData.Shippings.Add(newShipping);
            });
        });
        connection.On("RemoveShipping", (ulong shippingId) => {
            hubData.Shippings.Remove(hubData.Shippings.FirstOrDefault(s => s.Id == shippingId)!);
        });

        connection.On("ReceiveIncident", async (Incident newIncident) => {
            await Task.Run(() =>
            {
                var oldIncident = hubData.Incidents.FirstOrDefault(s => s.Id == newIncident.Id);
                if (oldIncident is not null)
                    Helper.ChangeAllProperties(oldIncident, newIncident);
                else
                    //Это вообще как должно случится?
                    throw new Exception("Как так-то");
            });
        });
        
        connection.On("ReceiveChatMember", (ulong chatId, User chatMember) => {
            hubData.Chats.GetChatData(chatId)?.members.Add(chatMember);
        });
        connection.On("RemoveChatMember", (ulong chatId, Guid chatMemberId) => {
            var members = hubData.Chats.GetChatData(chatId)?.members;
            members?.Remove(members.FirstOrDefault(m => m.Id == chatMemberId)!);
        });
        
        connection.On("RemoveChat", (ulong chatId) => {
            hubData.Chats.RemoveChat(chatId);
        });
        
        return connection;
    }

    public async Task Load()
    {
        await AwaitForConnection();
        var shippings = await GetShippings();
        hubData.Shippings = [..shippings];
        hubData.Incidents = [.. await GetIncidents()];
        foreach (var chat in await GetChats())
            hubData.Chats.TryAdd(chat, ([.. await GetChatMembers(chat.Id)], [.. await GetChatMessages(chat.Id)]));
        hubData.Shifts = [.. await GetDriversShifts()];
    }

    public async Task<IEnumerable<User>?> GetChatMembers(ulong chatId)
    {
        var chatMembers = await GetSomething<IEnumerable<User>>("GetChatMembers", chatId);
        var enumerable = chatMembers?.ToArray();
        
        if (enumerable is not null)
        {
            var chat = hubData.Chats.FirstOrDefault(c => c.Key.Id == chatId).Value;
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
             var chatInfo = hubData.Chats.FirstOrDefault(c => c.Key.Id == chatId).Value;
             chatInfo.messages = [.. messages];
        }

        return messages;
    }
    public async Task<IEnumerable<Incident>?> GetIncidents()
    {
        var incidents = (await GetSomething<IEnumerable<Incident>>("GetIncidents"))?.ToArray();
        if (incidents is not null)
        {
            hubData.Incidents = [.. incidents];
        }

        return incidents;
    }
    public async Task<IEnumerable<Shipping>?> GetShippings()
    {
        var shippings = (await GetSomething<IEnumerable<Shipping>>("GetShippings"))?.ToArray();
        if (shippings is not null)
        {
            hubData.Shippings = [.. shippings];
        }
        
        return shippings;
    }
    public async Task<IEnumerable<DriversShift>?> GetDriversShifts()
    {
        var shifts = (await GetSomething<IEnumerable<DriversShift>>("GetDriversShifts"))?.ToArray();
        if (shifts is null) return shifts;
        
        hubData.Shifts =  [..shifts];
        return shifts;
    }

    
    public async Task<Incident?> CreateIncident(Incident? incident)
    {
        incident = await GetSomething<Incident>("CreateIncident", incident);
        if (incident is not null)
        {
            hubData.Incidents.Add(incident);
        }

        return incident;
    }
    public async Task<User?> AddChatMember(Chat? chat, User? user)
    {
        user = await GetSomething<User>("AddChatMember", chat, user);
        if (user is not null && hubData.Chats.TryGetValue(chat!, out var chatInfo))
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
            hubData.Chats.TryAdd(chat, ([App.CurrentDriver.User], []));
        }

        return chat;
    }
    public async Task<Message?> SendMessage(Message? message)
    {
        message = await GetSomething<Message>("SendMessage", message);
        
        if (message is null) return message;
        
        var chatInfo = hubData.Chats.FirstOrDefault(c => c.Key.Id == message.ChatId).Value;
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
            hubData.Shifts.Add(shift);
        }
        return shift;
    }
    public async Task<DriversShift?> EndShift(DriversShift? shift)
    {
        var newShift = await GetSomething<DriversShift>("EndShift", shift);
        if (newShift is not null)
        {
            hubData.Shifts.InsertInsteadOf(shift!, newShift);
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
        var response = await httpClient.GetFromJsonAsync<Response>($"api/Auth/Authorize?login={login}&password={password}");

        var userAuth = JsonConvert.DeserializeObject<UserAuth>(response!.Data!.ToString()!);
        
        if (userAuth is null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка авторизации",  $"При авторизации произошла ошибка и сервер вернул: {JsonSerializer.Serialize(userAuth)}")
                .ShowAsync();
            return null;
        }

        _hub = CreateConnection(bearerToken: userAuth.Token);
        await StartConnection();
        
        return userAuth;
    }


    private async Task<T?> GetSomething<T>(string methodName, params object?[]? parameters)
    {
        var model = await (parameters?.Length switch
        {
            1 => _hub.InvokeAsync<T>(methodName, parameters[0]),
            2 => _hub.InvokeAsync<T>(methodName, parameters[0], parameters[1]),
            3 => _hub.InvokeAsync<T>(methodName, parameters[0], parameters[1], parameters[2]),
            _ => _hub.InvokeAsync<T>(methodName)
        });

        if (model is null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка получения данных с сервера",  $"При вызове {methodName} произошла ошибка и сервер вернул: {JsonSerializer.Serialize(model)}")
                .ShowAsync();
        }

        return model;
    }

    private async Task AwaitForConnection()
    {
        while (_hub?.State != HubConnectionState.Connected)
        {
            await Task.Delay(250);
        }
    }
}