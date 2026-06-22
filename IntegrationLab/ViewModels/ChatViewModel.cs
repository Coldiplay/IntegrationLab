using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BaseLibrary.Model.Classes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Model;
using IntegrationLab.Views;

namespace IntegrationLab.ViewModels;

public partial class ChatViewModel : ViewModelControlBase<ChatView>
{
    public ChatViewModel(ChatView view, IHubHandler hub, HubData hubData) : base(view)
    {
        _hubData = hubData;
        _hubHandler = hub;
        View.Initialized += (sender, args) => { OnPropertyChanged(nameof(Messages)); };
        View.GettingFocus += (sender, args) => {OnPropertyChanged(nameof(Messages)); };
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

    private readonly HubData _hubData;
    private readonly IHubHandler _hubHandler;

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(SendMessageCommand))] 
    public partial string MessageText { get; set; } = string.Empty;

    private bool CanSendMessage()
    {
        return !string.IsNullOrWhiteSpace(MessageText);
    }
    
    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    private async Task SendMessage(string text)
    {

        var message = new Message
        {
            Sender = App.CurrentDriver.User,
            Chat = Chat,
            ChatId = Chat.Id,
            Content = text,
            CreatedAt = DateTime.Now,
            SenderId = App.CurrentDriver.User.Id
        };

        await _hubHandler.SendMessage(message);
        
        Messages.Add(message);
        
        MessageText = string.Empty;
        OnPropertyChanged(nameof(Messages));
    }

    [RelayCommand]
    private static void ReturnToChatList()
    {
        App.ChangeCurrentView<MainViewModel>();
    }
}