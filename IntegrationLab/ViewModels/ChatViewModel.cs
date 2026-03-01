using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Model;
using IntegrationLab.Views;
using Microsoft.AspNetCore.SignalR.Client;
using Models.Model;

namespace IntegrationLab.ViewModels;

public partial class ChatViewModel : ViewModelControlBase<ChatView>
{
    [ObservableProperty]
    private Chat _chat;
    
    [ObservableProperty]
    private ObservableCollection<Message> _messages = [];

    private HubConnection _hub;
    private HttpClient _httpClient;

    public ChatViewModel()
    {
        View.Initialized += LoadMessages;
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
        await _hub.SendAsync("SendMessage", message);
    }
}