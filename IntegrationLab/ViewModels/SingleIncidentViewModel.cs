using BaseLibrary.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationLab.ViewModels;

public partial class SingleIncidentViewModel : ViewModelControlBase<SingleIncidentView>
{
    [ObservableProperty]
    public partial Incident Incident { get; set; } = null!;
    
    [RelayCommand]
    private static void ReturnToIncidentsList() =>
        App.ChangeCurrentView(App.Services.GetRequiredService<MainViewModel>());
}