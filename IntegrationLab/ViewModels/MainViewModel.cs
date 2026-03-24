using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationLab.ViewModels;

public partial class MainViewModel : ViewModelControlBase<MainView>
{
    [RelayCommand]
    private void OpenShippingsView() =>
        CurrentView = App.Services.GetRequiredService<ShippingsView>();
    [RelayCommand]
    private void OpenChatListView() =>
        CurrentView = App.Services.GetRequiredService<ChatListView>();
    [RelayCommand]
    private void OpenIncidentsView() =>
        CurrentView = App.Services.GetRequiredService<IncidentsView>();
    [RelayCommand]
    private void OpenActiveShipping() =>
        CurrentView = App.Services.GetRequiredService<ActiveShippingView>();
    
    
    [ObservableProperty] private Control _currentView = App.Services.GetRequiredService<ShippingsView>();
    public override void OnCreating()
    {
        
    }
}