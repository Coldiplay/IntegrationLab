using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Model;
using IntegrationLab.Views;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Models.Model;

namespace IntegrationLab.ViewModels;

public partial class ChatListViewModel : ViewModelControlBase<ChatListView>
{
    public ChatListViewModel()
    {
        View.GotFocus += (sender, args) =>
        {
            ConnectHub();
        };
    }

    private HubConnection? _hub;
    private HubData? _hubData;

    public ObservableCollection<Chat>? Chats => _hubData?.Chats;
    
    private void ConnectHub()
    {
        if (_hub is not null)
        {
            if (_hub.State == HubConnectionState.Disconnected)
                _hub.StartAsync();
            return;
        }
        
        _hub = new HubConnectionBuilder()
            .WithAutomaticReconnect()
            .WithUrl(App.HUB_CONNECTION)
            .Build();

        _hubData = App.Services.GetRequiredService<HubData>();
        
        //_hubData.Chats = тут надо будет с httpclient взять все чаты 
        
        //Чтонибудь придумать
        _hub.On<Message>("ReceiveMessage", message =>
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                var chat = _hubData.Chats.FirstOrDefault(c => c.Id == message.ChatId);
                if (chat is not null) return;
                
                chat = new Chat()
                {
                    ReceiverId = App.CurrentDriver.UserId,
                    SenderId = message.SenderId
                };
                _hubData.Chats.Add(chat);
            });
        });
        
        _hub.StartAsync();
    }
    
    [RelayCommand]
    private static void OpenChat(Chat chat)
    {
        var chatView = App.Services.GetRequiredService<ChatView>();
        (chatView.DataContext as ChatViewModel)!.Chat = chat;
        App.CurrentView = chatView;
    }
}