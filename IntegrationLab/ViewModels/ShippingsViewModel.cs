using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Views;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Models.Model;
using Models.Tools;

namespace IntegrationLab.ViewModels;

public partial class ShippingsViewModel : ViewModelControlBase<ShippingsView>
{
    [ObservableProperty]
    private ObservableCollection<Shipping> _shippings = [];

    public ShippingsViewModel()
    {
        TestData();
    }
    
    public Shipping? SelectedShipping
    {
        get;
        set
        {
            if (Equals(value, field)) return;
            field = value;
            OnPropertyChanged();
            ConfirmShippingCommand.NotifyCanExecuteChanged();
        }
    }

    private HttpClient _client;
    //TODO: Сделать On для обновления рейсов
    private HubConnection _hub;
    
    private void TestData()
    {
        var faker = GlobalOptions.Faker;

        for (int i = 0; i < 10; i++)
        {
            var shippingOrder = new ShippingOrder()
            {
                OrderDate = DateTime.Now.AddDays(2),
                Address = faker.Address.FullAddress(),
                Id = i + 1
            };
            var cargos = new List<Cargo>();
            
            for (int j = 0; j < faker.Random.Int(1, 10); j++)
            {
                var cargoType = new CargoType()
                {
                    Id = faker.Random.Int(1),
                    Title = faker.Commerce.ProductMaterial()
                };
                
                cargos.Add(new Cargo
                {
                    CargoType = cargoType,
                    DangerLevel = DangerLevel.None,
                    Dimensions = new Dimensions()
                    {
                        Height = faker.Random.Double(1, 100),
                        Length = faker.Random.Double(1, 100),
                        Width = faker.Random.Double(1, 100)
                    },
                    Id = Guid.NewGuid(),
                    Name = faker.Commerce.ProductName(),
                });  
            }
            
            shippingOrder.AddRangeCargo(cargos);
            
            
            var shipping = new Shipping()
            {
                Cargos = shippingOrder.Cargos,
                DesignatedDriverId = App.CurrentDriver.UserId,
                EstimatedDeliveryDate = DateTime.Now.AddDays(7 + i),
                Id = Guid.NewGuid()
            };
            Shippings.Add(shipping);
        }
    }
    
    [RelayCommand]
    private void ConfirmShipping()
    {
        var shipping = SelectedShipping;
        if (shipping is null) return;
        //????
        new Thread(() =>
        {
            //shipping.Confirmed = true;
            shipping!.ConfirmedStatus = "Подтверждение...";
            //TODO: Потом заменить
            //_client.PatchAsJsonAsync("api/Shippings/Confirm", shipping.Id);
        }).Start();
    }

    private async Task LoadShippings()
    {
        Shippings = await _client.GetFromJsonAsync<ObservableCollection<Shipping>>("api/Shippings")
            ?? [];
    }
    
    public void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not StackPanel stackPanel) return;

        var shipping = ((stackPanel.Parent as ListBoxItem)!.Content as Shipping)!;
        OpenShippingCommand.Execute(shipping);
    }
    
    [RelayCommand]
    private static void OpenShipping(Shipping shipping)
    {
        var shippingView = App.Services.GetRequiredService<SingleShippingView>();
        (shippingView.DataContext as SingleShippingViewModel)!.Shipping = shipping;
        App.MainWindowViewModel.CurrentView = shippingView;
    }

    public override void OnCreating()
    {
        _client = App.Services.GetRequiredService<HttpClient>();

        //TestData();
    }
}