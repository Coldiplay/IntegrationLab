using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Views;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Models.Model;

namespace IntegrationLab.ViewModels;

public partial class ChatViewModel : ViewModelControlBase<ChatView>
{
    public override void OnCreating()
    {
        //View.Initialized += LoadMessages;
        View.Initialized += TestData;
    }
    
    [ObservableProperty]
    private Chat _chat;
    
    [ObservableProperty]
    private ObservableCollection<Message> _messages = [];

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

    private void TestData(object? sender, EventArgs e)
    {
        Messages.Add(new Message()
        {
            Chat = this.Chat,
            ChatId = this.Chat.Id,
            Content = "Ты опять выходишь на связь, а?",
            Date = DateTime.Now.AddSeconds(-20),
            Id = Guid.NewGuid(),
            Sender = this.Chat.Sender,
            SenderId = this.Chat.SenderId
        });
        OnPropertyChanged(nameof(Messages));
    }

    private async void LoadMessages(object? sender, EventArgs e)
    {
        Messages = 
            await _httpClient.GetFromJsonAsync<ObservableCollection<Message>>("api/messages") 
            ?? [];
    }

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
    }

    [RelayCommand]
    private static void ReturnToChatList() =>
        App.MainWindowViewModel.CurrentView = App.Services.GetRequiredService<MainView>();
}