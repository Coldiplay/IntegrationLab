using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using IntegrationLab.Model;
using IntegrationLab.ViewModels;
using IntegrationLab.Views;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Models.Model;

namespace IntegrationLab;

public partial class App : Application
{
    public static ServiceProvider Services { get; private set; }
    public static MainWindow MainWindow { get; set; }
    public static MainWindowViewModel MainWindowViewModel =>
            (MainWindow.DataContext as MainWindowViewModel)!;
    public static Control? CurrentView
    {
        get => MainWindowViewModel.CurrentView;
        set => MainWindowViewModel.CurrentView = value;
    }
    
    public static Driver CurrentDriver { get; set; }

    public const string HUB_CONNECTION = "https://localhost:5001";

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        BuildServices();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = MainWindow;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = Services.GetRequiredService<MainView>();
            // singleViewPlatform.MainView = new MainView
            // {
            //     DataContext = new MainViewModel()
            // };
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    private static void BuildServices()
    {
        var services = new ServiceCollection();
        //Добавляем сервисы
            
        //singleton т.к. будем всегда возвращаться на этот control
        services.AddSingleton<MainView>(_ => 
            Tools.Helper.InitializeView<MainView, MainViewModel>());

        services.AddSingleton<HubData>();
                                                        
        Services = services.BuildServiceProvider();
     
        MainWindow = new MainWindow();
        var mainWindowViewModel = new MainWindowViewModel(MainWindow);
        MainWindow.DataContext = mainWindowViewModel; 
        
        mainWindowViewModel.CurrentView = Services.GetRequiredService<MainView>();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}