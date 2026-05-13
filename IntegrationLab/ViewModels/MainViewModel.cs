using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Model;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationLab.ViewModels;

public partial class MainViewModel : ViewModelControlBase<MainView>
{
    public MainViewModel()
    {
        var test = App.Services.GetRequiredService<HubHandler>();
        test.Start();
        //Thread.Sleep(5000);
    }

[RelayCommand]
    private void OpenShippingsView()
    {
        CurrentView = App.Services.GetRequiredService<ShippingsViewModel>();
    }

    [RelayCommand]
    private void OpenChatListView()
    {
        CurrentView = App.Services.GetRequiredService<ChatListViewModel>();
    }

    [RelayCommand]
    private void OpenIncidentsView()
    {
        CurrentView = App.Services.GetRequiredService<IncidentsViewModel>();
    }


    [ObservableProperty]
    public partial ViewModelBase CurrentView { get; set; } = App.Services.GetRequiredService<ShippingsViewModel>();
}