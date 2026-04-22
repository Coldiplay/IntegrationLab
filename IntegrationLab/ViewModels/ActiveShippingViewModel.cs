using BaseLibrary.Model;
using BaseLibrary.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using IntegrationLab.Model;
using IntegrationLab.Views;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationLab.ViewModels;

public partial class ActiveShippingViewModel : ViewModelControlBase<ActiveShippingView>
{
    [ObservableProperty]
    public partial Shipping ActiveShipping { get; set; }

    public ActiveShippingViewModel()
    {
        //TODO: Убрать потом и раскомментить в OnCreating
        TestData();
    }
    
    
    private void TestData()
    {
        var hubData = App.Services.GetRequiredService<HubData>();
        ActiveShipping = hubData.Shippings[GlobalOptions.Faker.Random.Int(0, hubData.Shippings.Count - 1)];
    }
    
    public override void OnCreating()
    {
        //TestData();
    }
}