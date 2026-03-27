using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;
using BaseLibrary.Model;

namespace IntegrationLab.ViewModels;

public partial class SingleIncidentViewModel : ViewModelControlBase<SingleIncidentView>
{
    [ObservableProperty]
    private Incident _incident = null!;
    
    
    public override void OnCreating()
    {
        
    }
    
    [RelayCommand]
    private static void ReturnToIncidentsList() =>
        App.CurrentView = App.Services.GetRequiredService<MainView>();
}