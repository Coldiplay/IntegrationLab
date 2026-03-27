using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Model;
using IntegrationLab.Views;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using BaseLibrary.Model;

namespace IntegrationLab.ViewModels;

public partial class ChatViewModel : ViewModelControlBase<ChatView>
{
    public override void OnCreating()
    {
        //View.Initialized += LoadMessages;
        View.Initialized += (sender, args) =>
        {
            OnPropertyChanged(nameof(Messages));    
        };
        
    }
    
    [ObservableProperty]
    private Chat _chat;

    public ObservableCollection<Message> Messages
    {
        get
        {
            if (Chat is null) return [];
            
            _hubData.Chats.TryGetValue(Chat, out var cartege);
            return cartege.Item2;
        }
    }
        //= HubD;

    //[ObservableProperty]
    private readonly HubData _hubData = App.Services.GetRequiredService<HubData>();
    private HubConnection _hub;
    private HttpClient _httpClient;

    [ObservableProperty] 
    private string _messageText = string.Empty;
    
    /*
    // private HorizontalAlignment GetAlignmentForMessage(Message message)
    //     => message.SenderId == App.CurrentDriver.UserId
    //     ? HorizontalAlignment.Right
    //     : HorizontalAlignment.Left;
    */
    

    // private async void LoadMessages(object? sender, EventArgs e)
    // {
    //     Messages = 
    //         await _httpClient.GetFromJsonAsync<ObservableCollection<Message>>("api/messages") 
    //         ?? [];
    // }

    [RelayCommand]
    private async Task SendMessage(string message)
    {
        //Вообще другая проверка нужна, но и так сойдёт :)
        if (string.IsNullOrWhiteSpace(message)) return;
        
        //TODO: Потом добавить обратно
        //await _hub.SendAsync("SendMessage", message);

        Messages.Add(new Message()
        {
            Sender = App.CurrentDriver.User,
            Chat = this.Chat,
            ChatId = this.Chat.Id,
            Content = message,
            Date = DateTime.Now,
            Id = Guid.NewGuid(),
            SenderId = App.CurrentDriver.User.Id
        });
        MessageText = string.Empty;
        OnPropertyChanged(nameof(Messages));
    }

    [RelayCommand]
    private static void ReturnToChatList() =>
        App.ChangeCurrentView(App.Services.GetRequiredService<MainView>());
}