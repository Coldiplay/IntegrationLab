using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationLab.ViewModels;

public partial class MainWindowViewModel(MainWindow mainWindow) : ViewModelBase
{
    public MainWindowViewModel() : this(App.MainWindow) { }

    public MainWindow MainWindow { get; private set; } = mainWindow;

    // Потом убрать инициализацию поля
    [ObservableProperty] private Control? _currentView = App.Services.GetRequiredService<MainView>();
}