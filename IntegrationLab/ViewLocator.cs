using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using IntegrationLab.ViewModels;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationLab;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    private readonly IServiceProvider _services;
    public ViewLocator(IServiceProvider services)
    {
        _services = services;
    }
    
    /*
     Old Approach
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }
    */
    public Control Build(object? data)
    {
        if (data is not ViewModelBase viewModel) return new TextBlock { Text = $"No view for {data?.GetType().Name}" };
        
        Control view = viewModel switch
        {
            ActiveShippingViewModel => _services.GetRequiredService<ActiveShippingView>(),
            ChatListViewModel => _services.GetRequiredService<ChatListView>(),
            ChatViewModel => _services.GetRequiredService<ChatView>(),
            CreateIncidentViewModel => _services.GetRequiredService<CreateIncidentView>(),
            IncidentsViewModel => _services.GetRequiredService<IncidentsView>(),
            MainViewModel => _services.GetRequiredService<MainView>(),
            MainWindowViewModel => _services.GetRequiredService<MainWindow>(),
            ShippingsViewModel => _services.GetRequiredService<ShippingsView>(),
            SingleIncidentViewModel => _services.GetRequiredService<SingleIncidentView>(),
            SingleShippingViewModel => _services.GetRequiredService<SingleShippingView>(),
            
            _ => new TextBlock { Text = $"No view for {data.GetType().Name}" }
        };
        if (viewModel is not MainWindowViewModel) viewModel.View = view;

        return view;
    }

    public bool Match(object? data) => data is ViewModelBase;
}