using CommunityToolkit.Mvvm.ComponentModel;
using IntegrationLab.Views;

namespace IntegrationLab.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(MainWindow mainWindow)
    {
        MainWindow = mainWindow;
    }

    public MainWindow MainWindow { get; private set; }
    
    [ObservableProperty] public partial ViewModelBase CurrentPage { get; set; } = App.CurrentView;
}