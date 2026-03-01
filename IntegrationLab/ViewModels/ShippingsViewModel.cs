using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Views;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Models.Model;

namespace IntegrationLab.ViewModels;

public partial class ShippingsViewModel : ViewModelControlBase<ShippingsView>
{
    [ObservableProperty]
    private ObservableCollection<Shipping> _shippings = [];

    private readonly HttpClient _client;
    //TODO: Сделать On для обновления рейсов
    private HubConnection _hub;

    public ShippingsViewModel()
    {
        _client = App.Services.GetRequiredService<HttpClient>();

        TestData();
    }

    private void TestData()
    {
        var shippingOrder = new ShippingOrder()
        {
            OrderDate = DateTime.Now.AddDays(2),
            Address = "Куда-нибудь",
            Id = 1
        };
        var cargos = new List<Cargo>() 
        {
            new()
            {
                CargoType = new CargoType()
                {
                    Id = 1,
                    Title = "Насыпной"
                },
                DangerLevel = DangerLevel.None,
                Dimensions = new Dimensions()
                {
                    Height = 20,
                    Length = 40,
                    Width = 40
                },
                Id = Guid.NewGuid(),
                Name = "Песок",
            }
        };
        shippingOrder.AddRangeCargo(cargos);

        var shipping = new Shipping()
        {
            Cargos = shippingOrder.Cargos,
            DesignatedDriverId = App.CurrentDriver.UserId,
            EstimatedDeliveryDate = DateTime.Now.AddDays(7),
            Id = Guid.NewGuid()
        };
        
        Shippings =
        [
            shipping
        ];
    }

    [ObservableProperty]
    private bool _true = true;
    [RelayCommand(CanExecute = nameof(True))]
    private void ConfirmShipping(Shipping shipping)
    {
        //????
        new Thread(() =>
        {
            //shipping.Confirmed = true;
            shipping.ConfirmedStatus = "Подтверждение...";
            //TODO: Потом заменить
            //_client.PatchAsJsonAsync("api/Shippings/Confirm", shipping.Id);
        }).Start();
    }

    private async Task LoadShippings()
    {
        Shippings = await _client.GetFromJsonAsync<ObservableCollection<Shipping>>("api/Shippings")
            ?? [];
    }

    public override void OnCreating()
    {
    }
}