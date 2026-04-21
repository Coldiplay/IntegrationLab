using System;
using BaseLibrary.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using IntegrationLab.Model;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationLab.ViewModels;

public partial class CreateIncidentViewModel : ViewModelControlBase<CreateIncidentView>
{
    [ObservableProperty] public partial Shipping? Shipping { get; set; } = App.Services.GetRequiredService<HubData>().ActiveShipping;
    [ObservableProperty] public partial TimeSpan Time { get; set; } = DateTime.Now.TimeOfDay;
    
    public override void OnCreating()
    {
        
    }
}