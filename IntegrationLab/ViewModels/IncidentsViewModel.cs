using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Model;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;
using BaseLibrary.Model;

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
        if (sender is not StackPanel stackPanel) return;

        var incident = ((stackPanel.Parent as ListBoxItem)!.Content as Incident)!;
        
        OpenIncidentViewCommand.Execute(incident);
    }

    [RelayCommand]
    private static void OpenIncidentView(Incident incident)
    {
        var incidentView = App.Services.GetRequiredService<SingleIncidentView>();
        (incidentView.DataContext as SingleIncidentViewModel)!.Incident = incident;
        App.CurrentView = incidentView;
    }
    
    public override void OnCreating()
    {
        //TestData();
        OnPropertyChanged(nameof(Incidents));
    }
}