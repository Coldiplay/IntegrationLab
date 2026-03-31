using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLibrary.Model;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

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
                        [.. await GetMessages(newMessage.ChatId)]
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
                    Tools.Helper.ChangeAllProperties(oldIncident, newIncident);
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
                _hubData.Chats.TryAdd(chat, ([.. await GetChatMembers(chat.Id)], [.. await GetMessages(chat.Id)]));
            }
        });
    }

    public async Task<IEnumerable<User>> GetChatMembers(int chatId)
    {
        return await SimpleGet<IEnumerable<User>>("GetChatMembers", chatId);
    }
    
    public async Task<IEnumerable<Chat>> GetChats(int? userId = null) => 
        await SimpleGet<IEnumerable<Chat>>("GetChats", CheckUserId(userId));

    public async Task<IEnumerable<Message>> GetMessages(int chatId) =>
        await SimpleGet<IEnumerable<Message>>("GetMessages", chatId);

    public async Task<IEnumerable<Incident>> GetIncidents(int? userId = null) =>
        await SimpleGet<IEnumerable<Incident>>("GetIncidents", CheckUserId(userId));

    public async Task<IEnumerable<Shipping>> GetShippings(int? userId = null)
    {
        return await SimpleGet<IEnumerable<Shipping>>("GetShippings", CheckUserId(userId));
    }

    
    private async Task<T> SimpleGet<T>(string methodName)
    {
        return await _hub.InvokeAsync<T>(methodName);
    }
    private async Task<T> SimpleGet<T>(string methodName, object parameter)
    {
        return await _hub.InvokeAsync<T>(methodName, parameter);
    }
    

    private static int CheckUserId(int? userId) => userId is null or < 1 ? App.CurrentDriverId : userId.Value;

}