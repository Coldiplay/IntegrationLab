using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
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
    public override void OnCreating()
    {
        TestData();
        // View.GotFocus += (sender, args) =>
        // {
        //     ConnectHub();
        // };
    }

    private void TestData()
    {
        _hubData = new HubData();
        var sender = new User()
        {
            Id = 95,
            Login = "CoolSkeleton95",
            Name = "Test",
            LastName = "Test",
        };
        _hubData.Chats = [
            new Chat()
            {
                Id = 1,
                Receiver = App.CurrentDriver.User,
                ReceiverId = App.CurrentDriver.UserId,
                Sender = sender,
                SenderId = sender.Id
            }
        ];
        OnPropertyChanged(nameof(Chats));
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
                
                //message.GetMessageHorizontalAlignment;
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
    
    public void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not StackPanel stackPanel) return;

        var chat = ((stackPanel.Parent as ListBoxItem)!.Content as Chat)!;
        OpenChatCommand.Execute(chat);
    }
    
    [RelayCommand]
    private static void OpenChat(Chat chat)
    {
        var chatView = App.Services.GetRequiredService<ChatView>();
        (chatView.DataContext as ChatViewModel)!.Chat = chat;
        ((App.MainWindowViewModel.CurrentView as MainView)!.DataContext as MainViewModel)!.CurrentView = chatView;
    }
}