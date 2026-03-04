using System;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationLab.ViewModels;

public partial class MainWindowViewModel(MainWindow mainWindow) : ViewModelBase
{
    public MainWindowViewModel() : this(App.MainWindow) { }

    public MainWindow MainWindow { get; private set; } = mainWindow;
    
    // public Action TestAction { get; set; } = () =>
    // {
    //     App.MainViewModel!.OpenShippingsViewCommand.Execute(null);
    // };
    
    [RelayCommand]
    private static void InvokeOnUIThread(Action action) => 
        Dispatcher.UIThread.Invoke(action);
    
    // Потом убрать инициализацию поля
    [ObservableProperty] 
    private Control? _currentView = App.Services.GetRequiredService<MainView>();
}