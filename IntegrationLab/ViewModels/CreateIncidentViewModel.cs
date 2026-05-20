using System;
using System.Threading.Tasks;
using BaseLibrary.Model.Classes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Model;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationLab.ViewModels;

public partial class CreateIncidentViewModel : ViewModelControlBase<CreateIncidentView>
{
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))]
    public partial Shipping? Shipping { get; set; } = App.Services.GetRequiredService<HubData>().ActiveShipping;
    [ObservableProperty] public partial TimeSpan Time { get; set; } = DateTime.Now.TimeOfDay;
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))] 
    public partial string? Description { get; set; } = string.Empty;
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(CreateIncidentCommand))] 
    private partial bool CreateBlock { get; set; } = false;

    [ObservableProperty] public partial string CreateButtonContent { get; set; } = "Отправить";

    [RelayCommand(CanExecute = nameof(CanCreateIncident))]
    private async Task CreateIncident()
    {
        CreateBlock = true;
        var hub = App.Services.GetRequiredService<HubHandler>();
        var incident = await hub.CreateIncident(new Incident()
        {
            Description = Description!,
            DriverId = App.CurrentDriverId,
            IncidentDate =
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddSeconds(Time.TotalSeconds),
            ShippingId = Shipping!.Id
        });
        if (incident is not null)
        {
            CreateButtonContent = "Отправлено";
            await Task.Delay(1500);
            ReturnToIncidentsListCommand.Execute(null);
        }
        CreateBlock = false;
    }

    private bool CanCreateIncident()
    {
        return !CreateBlock 
               && Shipping is not null 
               && string.IsNullOrWhiteSpace(Description) 
               && Description!.Length > 8;
    }
    
    [RelayCommand]
    private static void ReturnToIncidentsList() => 
        App.ChangeCurrentView<MainViewModel>();
}