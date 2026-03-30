using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLibrary.Model;
using Microsoft.AspNetCore.SignalR.Client;

namespace IntegrationLab.Model;

public class HubHandler
{
    // private static HubConnection _connection;
    // private static HubData _hubData;
    public static async Task<HubConnection> Initialize(HubConnectionBuilder builder, HubData hubData)
    {
        var connection = builder.WithAutomaticReconnect().WithUrl(App.HUB_CONNECTION).Build();
        
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
                        [.. await connection.InvokeAsync<List<User>>("GetChatMembers", newMessage.ChatId)],
                        [.. await connection.InvokeAsync<List<Message>>("GetMessages", newMessage.ChatId)]
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
        
        return connection;
    }
}