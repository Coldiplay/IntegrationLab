using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
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
        var shippingOrder = new ShippingOrder()
        {
            OrderDate = DateTime.Now.AddDays(2),
            Address = "Куда-нибудь",
            Id = 1
        };
        var cargos = new List<Cargo>();
        var cargoType = new CargoType()
        {
            Id = 1,
            Title = "Насыпной"
        };
        for (int i = 1; i <= 10; i++)
        {
            cargos.Add(new Cargo
            {
                CargoType = cargoType,
                DangerLevel = DangerLevel.None,
                Dimensions = new Dimensions()
                {
                    Height = 20,
                    Length = 40,
                    Width = 40
                },
                Id = Guid.NewGuid(),
                Name = "Песок",
            });
        }
        
        shippingOrder.AddRangeCargo(cargos);

        for (int i = 1; i <= 6; i++)
        {
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

    public override void OnCreating()
    {
        _client = App.Services.GetRequiredService<HttpClient>();

        TestData();
    }
}