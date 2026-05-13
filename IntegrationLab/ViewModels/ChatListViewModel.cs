using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using BaseLibrary.Model;
using BaseLibrary.Tools;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Model;
using IntegrationLab.Views;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationLab.ViewModels;

public partial class ChatListViewModel : ViewModelControlBase<ChatListView>
{
    public ChatListViewModel()
    {
        _hubData = App.Services.GetRequiredService<HubData>();
    }

    public override void OnCreating()
    {
        // View.GotFocus += (sender, args) =>
        // {
        //     ConnectHub();
        // };
    }

    private HubConnection? _hub;
    private HubData? _hubData;

    public ObservableCollection<Chat>? Chats => [.. _hubData?.Chats.Keys];

    public void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Control control
            || (control.Parent as ListBoxItem)?.Content is not Chat chat)
            return;
        OpenChatCommand.Execute(chat);
    }

    [RelayCommand]
    private static void OpenChat(Chat chat)
    {
        var chatView = App.Services.GetRequiredService<ChatViewModel>();
        chatView.Chat = chat;
        App.ChangeCurrentView(chatView);
    }
}