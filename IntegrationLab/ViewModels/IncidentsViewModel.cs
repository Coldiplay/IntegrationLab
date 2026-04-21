using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using BaseLibrary.Model;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Model;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationLab.ViewModels;

public partial class IncidentsViewModel : ViewModelControlBase<IncidentsView>
{
    public ObservableCollection<Incident> Incidents => _hubData.Incidents;
    

    private readonly HubData _hubData = App.Services.GetRequiredService<HubData>();
    public IncidentsViewModel()
    {
        //TestData();
    }
    
    public void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Control control
            || (control.Parent as ListBoxItem)?.Content is not Incident incident) return;
        OpenIncidentViewCommand.Execute(incident);
    }

    [RelayCommand]
    private static void OpenIncidentView(Incident incident)
    {
        var incidentView = App.Services.GetRequiredService<SingleIncidentViewModel>();
        incidentView.Incident = incident;
        App.ChangeCurrentView(incidentView);
    }

    [RelayCommand]
    private static void OpenCreateIncidentView()
    {
        App.ChangeCurrentView<CreateIncidentViewModel>();
    }
    
    public override void OnCreating()
    {
        //TestData();
        OnPropertyChanged(nameof(Incidents));
    }
}