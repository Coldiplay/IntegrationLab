using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using BaseLibrary.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Model;
using IntegrationLab.Views;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationLab.ViewModels;

public partial class ChatViewModel : ViewModelControlBase<ChatView>
{
    public ChatViewModel(ChatView view) : base(view)
    {
        View.Initialized += (sender, args) =>
        {
            OnPropertyChanged(nameof(Messages));    
        };
    }

    [ObservableProperty] public partial Chat Chat { get; set; }

    public ObservableCollection<Message> Messages
    {
        get
        {
            if (Chat is null) return [];
            
            _hubData.Chats.TryGetValue(Chat, out var cartege);
            return cartege.messages;
        }
    }
    
    private readonly HubData _hubData = App.Services.GetRequiredService<HubData>();
    private HubConnection _hub;
    private HttpClient _httpClient;

    [ObservableProperty] public partial string MessageText { get; set; } = string.Empty;

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
        App.ChangeCurrentView<MainViewModel>();
}