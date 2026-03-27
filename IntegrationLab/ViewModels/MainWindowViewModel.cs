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
    public MainWindow MainWindow { get; private set; } = mainWindow;
    
    [RelayCommand]
    private static void InvokeOnUIThread(Action action) => 
        Dispatcher.UIThread.Invoke(action);
    
    [ObservableProperty] private Control _currentView = App.CurrentView;
}