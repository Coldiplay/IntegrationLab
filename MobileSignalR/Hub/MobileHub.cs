using System.Net;
using BaseLibrary.Model.Classes;
using BaseLibrary.Tools;
using Microsoft.AspNetCore.SignalR;
using MobileSignalR.MiddleWares;
using MobileSignalR.Tools;
using Newtonsoft.Json;
using SignalRSwaggerGen.Attributes;

namespace MobileSignalR.Hub;

[SignalRHub("/hub")]
//[Authorize(Policy = "Authorized")]
public class MobileHub(LaravelRequestHandler laraClient, ConnectionsHandler connections, ILogger<MobileHub> logger) 
    : Microsoft.AspNetCore.SignalR.Hub
{
    // Мобилка <-> SignalR <<-> API (Laravel) <->> Сайт (Laravel)
    
    private string UserIdentity => $"{Context.ConnectionId} {Context.UserIdentifier} {Context.User?.Identity?.Name}";

    public async Task<IEnumerable<User>?> GetChatMembers(int chatId) =>
        await Get<IEnumerable<User>>($"api/chat/{chatId}/members");

    //TODO: Сделать пагинацию
    public async Task<IEnumerable<Message>?> GetChatMessages(int chatId) =>
        await Get<IEnumerable<Message>>($"api/chat/{chatId}/messages");

    public async Task<IEnumerable<Chat>?> GetChats() =>
        await Get<IEnumerable<Chat>>("api/chat/");

    public async Task<IEnumerable<Incident>?> GetIncidents() =>
        await Get<IEnumerable<Incident>>("api/incident/");

    public async Task<IEnumerable<Shipping>?> GetShippings() =>
        await Get<IEnumerable<Shipping>>("api/shipping/");

    public async Task<Message?> SendMessage(Message message)
    {
        var sentMessage = await Post<Message>($"api/chat/{message.ChatId}/messages", message);

        if (sentMessage is null) return sentMessage;
        
        await Clients.OthersInGroup("Chat " + message.ChatId).SendAsync("ReceiveMessage", sentMessage);

        return sentMessage;
    }

    public async Task<Incident?> CreateIncident(Incident incident)
    {
        var newIncident = await Post<Incident>("api/incident/", incident);

        if (newIncident is null) return newIncident;
        
        await AddUserToGroup(GetCurrentUserId()!.Value, "Incident" + newIncident.Id);

        return newIncident;
    }
    
    
    private async Task<T?> Get<T>(string url) where T : notnull
    {
        var token = GetAuthToken();
        var model = await laraClient.Get<T>(url, token);
        return model;
    }

    private async Task<T?> Post<T>(string url, object parameter) where T: notnull
    {
        var token = GetAuthToken();
        var model = await laraClient.Post<T>(url, parameter, token);
        return model;
    }

    private string? GetAuthToken()
    {
        var token = Context.GetHttpContext()?.Request.Headers.Authorization.ToString();
        if (token?.Length > 8) 
            token = token.Remove(0, 7);
        return token;
    }
    

    private Response ToBadResponse(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new Response
        {
            StatusCode = statusCode,
            Message = message
        };
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            connections.AddConnection(userId.Value, Context.ConnectionId);
            AddConnectionToChatGroups(userId.Value, Context.ConnectionId);
        }
        
        await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue) connections.RemoveConnection(userId.Value, Context.ConnectionId);
        
        return base.OnDisconnectedAsync(exception);
    }

    private async void AddConnectionToChatGroups(Guid userId, string connectionId)
    {
        while (true)
        {
            try
            {
                var chats = (await GetChats())?.ToList();
                if (chats is null)
                    throw new CustomException() {
                        ErrorMessage = "Response from api was null",
                        ErrorCode = 500
                    };
                
                foreach (var chat in chats)
                {
                    await Groups.AddToGroupAsync(connectionId, "Chat " + chat.Id);
                }
                logger.LogInformation("User {userId} was successfully added to all their chat groups", userId);

                return;
            }
            catch (Exception e)
            {
                logger.LogError("Adding user {userId} to chat groups failed. Error message: {message}", userId, e.Message);
                await Task.Delay(15000);
            }
        }
    }

    private async Task AddUserToGroup(Guid userId, string groupName)
    {
        var connectionsList = connections.GetConnections(userId);
        if (connectionsList is null) return;

        foreach (var connection in connectionsList)
        {
            await Groups.AddToGroupAsync(connection, groupName);
        }
    }

    private Guid? GetCurrentUserId()
    {
        return Guid.TryParse(Context.User?.Claims.FirstOrDefault(c => c.Type == "ID")?.Value, out var id)
            ? id
            : null;
    }
}