using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationLab.ViewModels;

public partial class MainViewModel : ViewModelControlBase<MainView>
{
    //[ObservableProperty] private string _greeting = "Welcome to Avalonia!";

    public MainViewModel()
    {
        
    }

    private string? Test => View.Name;

    [RelayCommand]
    private void OpenShippingsView() =>
        CurrentView = App.Services.GetRequiredService<ShippingsView>();
    [RelayCommand]
    private void OpenChatListView() =>
        CurrentView = App.Services.GetRequiredService<ChatListView>();
    
    
    [ObservableProperty] private Control _currentView = App.Services.GetRequiredService<ShippingsView>();
    public override void OnCreating()
    {
        
    }
}