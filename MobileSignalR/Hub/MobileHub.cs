using System.Net;
using BaseLibrary.Model.Classes;
using BaseLibrary.Tools;
using Microsoft.AspNetCore.SignalR;
using MobileSignalR.Tools;
using SignalRSwaggerGen.Attributes;

namespace MobileSignalR.Hub;

[SignalRHub("/hub")]
//[Authorize(Policy = "Authorized")]
public class MobileHub : Microsoft.AspNetCore.SignalR.Hub
{
    // Мобилка <-> SignalR <<-> API (Laravel) <->> Сайт (Laravel)
    public MobileHub(LaravelRequestHandler laraClient, ILogger<MobileHub> logger)
    {
        _laraClient = laraClient;
        _logger = logger;
    }
    
    private readonly LaravelRequestHandler _laraClient;
    private readonly ILogger _logger;
    
    private string UserIdentity => $"{Context.ConnectionId} {Context.UserIdentifier} {Context.User?.Identity?.Name}";

    public async Task<Response> GetChatMembers(int chatId) =>
        await Get<IEnumerable<User>>($"api/chat/{chatId}/members");

    //TODO: Сделать пагинацию
    public async Task<Response> GetChatMessages(int chatId) =>
        await Get<IEnumerable<Message>>($"api/chat/{chatId}/messages");

    public async Task<Response> GetChats() =>
        await Get<IEnumerable<Chat>>("api/chat/");

    public async Task<Response> GetIncidents() =>
        await Get<IEnumerable<Incident>>("api/incident/");

    public async Task<Response> GetShippings() =>
        await Get<IEnumerable<Shipping>>("api/shipping/");

    public async Task<Response> SendMessage(Message message)
    {
        var sentMessage = await Post<Message>($"api/chat/{message.ChatId}/messages", message);

        if ((int)sentMessage.StatusCode < 400)
        {
            
        }

        return sentMessage;
    }
    
    

    private async Task<Response> Get<T>(string url) where T : notnull
    {
        var token = GetAuthToken();
        var model = await _laraClient.Get<T>(url, token);
        return ToResponseWithData(model);
    }

    private async Task<Response> Post<T>(string url, object parameter) where T: notnull
    {
        var token = GetAuthToken();
        var model = await _laraClient.Post<T>(url, parameter, token);
        return ToResponseWithData(model);
    }

    private string? GetAuthToken()
    {
        var token = Context.GetHttpContext()?.Request.Headers.Authorization.ToString().Remove(0, 7);
        return token;
    }
    
    private Response ToResponseWithData<T>(T? model = default, string? message = null,
        HttpStatusCode statusCode = HttpStatusCode.OK)
        where T : notnull
    {
        if (model is null)
        {
            _logger.Log(LogLevel.Information, "");
            return new Response
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = message ?? "Not found"
            };
        }


        var returnType = typeof(T);
        var typeName = returnType.IsGenericType
            ? returnType.GetGenericArguments()[0].Name
            :  returnType.Name;
        return new Response
        {
            StatusCode = statusCode,
            Data = model,
            DataTypeName = typeName,
            Message = message ?? $"Successful retrieved {typeName}"
        };
    }

    private Response ToBadResponse(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new Response
        {
            StatusCode = statusCode,
            Message = message
        };
    }


    public override Task OnConnectedAsync()
    {
        //this.Context.ConnectionId;
        
        return base.OnConnectedAsync();
    }
}