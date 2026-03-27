using System;
using System.Linq;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using IntegrationLab.Model;
using IntegrationLab.Model.Db.Concrete;
using IntegrationLab.ViewModels;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;
using BaseLibrary.Model;
using BaseLibrary.Tools;

namespace IntegrationLab;

public partial class App : Application
{
    public static ServiceProvider Services { get; private set; }
    public static Control CurrentView { get; set; }
    public static Control PlatformMainIntefrace { get; set; }

    public static MainViewModel? MainViewModel => ((CurrentView as MainView)?.DataContext as MainViewModel);
    
    public static Driver CurrentDriver { get; set; }

    public const string HUB_CONNECTION = "https://localhost:5001";

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            BuildServices();
            CurrentView = Services.GetRequiredService<MainView>();
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            var window = new MainWindow();
            window.DataContext = new MainWindowViewModel(window);
            desktop.MainWindow = window;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            BuildServices(true);
            CurrentView = Services.GetRequiredService<MainView>();
            singleViewPlatform.MainView = CurrentView;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void TestData()
    {
        var faker = GlobalOptions.Faker;
        var fName = faker.Name.FirstName();
        var lName = faker.Name.LastName();
        var user = new User()
        {
            Id = 1,
            Name = fName,
            LastName = lName,
            Phone = faker.Phone.PhoneNumber(), 
            Login = faker.Internet.UserName(fName, lName)
        };

        CurrentDriver = new Driver()
        {
            UserId = user.Id,
            User = user,
            Rights = Rights.A | Rights.B
        };
    }
    
    private static void BuildServices(bool singleViewApp = false)
    {
        var services = new ServiceCollection();
        //Добавляем сервисы
        services.AddSingleton<HttpClient>(_ =>
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(GlobalOptions.API_URI);
            return client;
        });
       
        RegisterDbServices(services);
        
        RegisterViews(services);
        
        services.AddSingleton<HubData>();
                                                        
        Services = services.BuildServiceProvider();
     
        TestData();
    }

    private static void RegisterDbServices(ServiceCollection services)
    {
         services.AddSingleton<ReadOnlySimpleDb>(serviceProvider => 
             new ReadOnlySimpleDb(serviceProvider.GetRequiredService<HttpClient>()));
         
    }

    private static void RegisterViews(ServiceCollection services)
    {
        //singleton т.к. будем всегда возвращаться на этот control
        services.AddSingleton<MainView>(_ => 
            Tools.Helper.InitializeView<MainView>());

        services.AddSingleton<ShippingsView>(_ =>
            Tools.Helper.InitializeView<ShippingsView>());
        
        services.AddSingleton<ChatListView>(_ =>
            Tools.Helper.InitializeView<ChatListView>());

        services.AddTransient<ChatView>(_ =>
            Tools.Helper.InitializeView<ChatView>());
        
        services.AddSingleton<IncidentsView>(_ => 
            Tools.Helper.InitializeView<IncidentsView>());
        
        services.AddTransient<SingleIncidentView>(_ => 
            Tools.Helper.InitializeView<SingleIncidentView>());

        services.AddTransient<SingleShippingView>(_ =>
            Tools.Helper.InitializeView<SingleShippingView>());
        
        services.AddSingleton<ActiveShippingView>(_ =>
            Tools.Helper.InitializeView<ActiveShippingView>());
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