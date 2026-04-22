using BaseLibrary.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationLab.ViewModels;

public partial class SingleShippingViewModel : ViewModelControlBase<SingleShippingView>
{
    [ObservableProperty]
    public partial Shipping Shipping { get; set; } = null!;

    public override void OnCreating()
    {
        
    }
    
    
    [RelayCommand]
    private static void ReturnToShippingsList() =>
        App.ChangeCurrentView(App.Services.GetRequiredService<MainViewModel>());
}