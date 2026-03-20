using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;
using Models.Model;

namespace IntegrationLab.ViewModels;

public partial class SingleShippingViewModel : ViewModelControlBase<SingleShippingView>
{
    [ObservableProperty]
    private Shipping _shipping = null!;
    
    public override void OnCreating()
    {
        
    }
    
    
    [RelayCommand]
    private static void ReturnToShippingsList() =>
        App.MainWindowViewModel.CurrentView = App.Services.GetRequiredService<MainView>();
}