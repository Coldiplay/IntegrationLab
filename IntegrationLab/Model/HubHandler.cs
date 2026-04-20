using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLibrary.Model;
using BaseLibrary.Tools;
using IntegrationLab.Tools;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;

namespace IntegrationLab.Model;

public class HubHandler
{
    private readonly HubConnection _hub;
    private readonly HubData _hubData;

    public HubHandler(string connectionString = App.HUB_CONNECTION)
    {
        _hubData = App.Services.GetRequiredService<HubData>();
        _hub = Initialize(_hubData, connectionString);
        Start();
    }

    public async void Start()
    {
        await _hub.StartAsync();
    }

    public HubConnection Initialize(HubData hubData, string connectionString)
    {
        var connection = new HubConnectionBuilder().WithAutomaticReconnect().WithUrl(connectionString).Build();

        connection.On("ReceiveMessage", async (Message newMessage) =>
        {
            if (hubData.Chats.TryGetValue(newMessage.Chat, out var tuple))
            {
                //lock (tuple.messages)
                //{
                tuple.messages.Add(newMessage);
                //}
            }
            else
            {
                hubData.Chats.TryAdd(newMessage.Chat,
                    (
                        [.. await GetChatMembers(newMessage.ChatId)],
                        [.. await GetChatMessages(newMessage.ChatId)]
                    ));
            }
        });

        connection.On("ReceiveShipping", async (Shipping newShipping) =>
        {
            await Task.Run(() =>
            {
                //lock (hubData.Shippings)
                //{
                var oldShipping = hubData.Shippings.FirstOrDefault(s => s.Id == newShipping.Id);
                if (oldShipping is not null)
                {
                    //lock (oldShipping)
                    //{
                    hubData.Shippings.InsertInsteadOf(oldShipping, newShipping);
                    //}
                }
                else
                {
                    hubData.Shippings.Add(newShipping);
                }
                //}
            });
        });

        connection.On("UpdateIncident", async (Incident newIncident) =>
        {
            await Task.Run(() =>
            {
                //lock (hubData.Incidents)
                //{
                var oldIncident = hubData.Incidents.FirstOrDefault(s => s.Id == newIncident.Id);
                if (oldIncident is not null)
                {
                    Helper.ChangeAllProperties(oldIncident, newIncident);
                }
                else
                {
                    //Это вообще как должно случится?
                    throw new Exception("Как так-то");
                }

                //}
            });
        });

        Load();
        return connection;
    }

    public async void Load()
    {
        await Task.Run(async () =>
        {
            _hubData.Shippings.AddRange(await GetShippings());
            _hubData.Incidents.AddRange(await GetIncidents());
            foreach (var chat in (await GetChats()))
            {
                _hubData.Chats.TryAdd(chat, ([.. await GetChatMembers(chat.Id)], [.. await GetChatMessages(chat.Id)]));
            }
        });
    }

    public async Task<IEnumerable<User>?> GetChatMembers(int chatId)
    {
        var response = await SimpleGet("GetChatMembers", chatId);
        return await HandleResponse<IEnumerable<User>>(response);
    }

    //TODO: Зачем я добавил userId?...
    public async Task<IEnumerable<Chat>?> GetChats(int? userId = null)
    {
        var response = await SimpleGet("GetChats", CheckUserId(userId));
        return await HandleResponse<IEnumerable<Chat>>(response);
    }

    public async Task<IEnumerable<Message>?> GetChatMessages(int chatId)
    {
        var response = await SimpleGet("GetChatMessages", chatId);
        return  await HandleResponse<IEnumerable<Message>>(response);
    }

    public async Task<IEnumerable<Incident>?> GetIncidents(int? userId = null)
    {
        var response = await SimpleGet("GetIncidents", CheckUserId(userId));
        return await HandleResponse<IEnumerable<Incident>>(response);
    }
    
    public async Task<IEnumerable<Shipping>?> GetShippings(int? userId = null)
    {
        var response = await SimpleGet("GetShippings", CheckUserId(userId));
        return await HandleResponse<IEnumerable<Shipping>>(response);
    }


    private async Task<Response> SimpleGet(string methodName)
    {
        return await _hub.InvokeAsync<Response>(methodName);
    }

    private async Task<Response> SimpleGet(string methodName, object parameter)
    {
        return await _hub.InvokeAsync<Response>(methodName, parameter);
    }


    private static async Task<T?> HandleResponse<T>(Response response)
    {
        if ((int)response.StatusCode < 400)
        {
            try
            {
                return (T)(response.Data ?? throw new NullReferenceException());
            }
            catch (Exception e)
            {
                //TODO: Создать обработку ошибки преобразования данных
                Console.WriteLine(e);
                throw;
            }
        }
        
        
        //TODO: Создать обработку ошибки получения данных
        await MessageBoxManager.GetMessageBoxStandard("Ошибка", response.Message).ShowAsync();
        
        return default;
    }


    private static int CheckUserId(int? userId) => userId is null or < 1 ? App.CurrentDriverId : userId.Value;

}