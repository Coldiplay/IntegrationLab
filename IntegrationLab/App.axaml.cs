using System;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BaseLibrary.Model;
using BaseLibrary.Tools;
using IntegrationLab.Model;
using IntegrationLab.ViewModels;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationLab;

public partial class App : Application
{
    public static ServiceProvider Services { get; private set; }
    public static ViewModelBase CurrentView { get; private set; }
    public static int CurrentDriverId => CurrentDriver.User.Id;
    public static Driver CurrentDriver { get; set; }
    public const string HUB_CONNECTION = "https://localhost:5001";

    private static bool _isAppWithSingleView = true;
    private static ISingleViewApplicationLifetime _platform;
    private static MainWindow? _window;
    private static ViewLocator _locator;
    
    public static void ChangeCurrentView(ViewModelBase viewModel)
    {
        CurrentView = viewModel;
        if (_isAppWithSingleView)
        {
            _platform.MainView = Services.GetRequiredService<ViewLocator>().Build(CurrentView);
        }
        else
        {
            (_window!.DataContext as MainWindowViewModel)!.CurrentPage = CurrentView;
        }
    }

    public static void ChangeCurrentView<TViewModel>() where TViewModel : ViewModelBase
    {
        ChangeCurrentView(Services.GetRequiredService<TViewModel>());
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _isAppWithSingleView = false;
            BuildServices();
            _locator = new ViewLocator(Services);
            DataTemplates.Add(_locator);
            _window = Services.GetRequiredService<MainWindow>();
            _window.DataContext = Services.GetRequiredService<MainWindowViewModel>();
            ChangeCurrentView<MainViewModel>();
            desktop.MainWindow = _window;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            _platform = singleViewPlatform;
            _isAppWithSingleView = true;
            BuildServices(true);
            _locator = new ViewLocator(Services);
            DataTemplates.Add(_locator);
            ChangeCurrentView<MainViewModel>();
            singleViewPlatform.MainView = _locator.Build(CurrentView);
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
            //TODO: ..... А зачем оно надо?..
            var client = new HttpClient();
            client.BaseAddress = new Uri(GlobalOptions.HUB_URI);
            return client;
        });
        
        RegisterViews(services);

        if (!singleViewApp)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
        }
        
        services.AddSingleton<HubData>();
        
        Services = services.BuildServiceProvider();
     
        TestData();
    }

    private static void RegisterViews(ServiceCollection services)
    {
        /*
         Old Approach
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

        services.AddTransient<CreateIncidentView>(_ =>
            Tools.Helper.InitializeView<CreateIncidentView>());
        */
        //Register views and vms
        services.AddSingleton<MainView>();
        services.AddSingleton<MainViewModel>();
        
        services.AddSingleton<ShippingsView>();
        services.AddSingleton<ShippingsViewModel>();
        
        services.AddSingleton<ChatListView>();
        services.AddSingleton<ChatListViewModel>();
        
        services.AddTransient<ChatView>();
        services.AddTransient<ChatViewModel>();
        
        services.AddSingleton<IncidentsView>();
        services.AddSingleton<IncidentsViewModel>();
        
        services.AddTransient<SingleIncidentView>();
        services.AddTransient<SingleIncidentViewModel>();
        
        services.AddTransient<SingleShippingView>();
        services.AddTransient<SingleShippingViewModel>();
        
        services.AddSingleton<ActiveShippingView>();
        services.AddSingleton<ActiveShippingViewModel>();
        
        services.AddTransient<CreateIncidentView>();
        services.AddTransient<CreateIncidentViewModel>();
    }
}