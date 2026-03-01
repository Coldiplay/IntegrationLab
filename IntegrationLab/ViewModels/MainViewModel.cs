using CommunityToolkit.Mvvm.ComponentModel;
using IntegrationLab.Model;
using IntegrationLab.Views;
using Models.Model;

namespace IntegrationLab.ViewModels;

public partial class MainViewModel : ViewModelControlBase<MainView>
{
    [ObservableProperty] private string _greeting = "Welcome to Avalonia!";

    private void Test()
    {
        var order = new ShippingOrder();
        //order.Add(new Cargo());
    }
}