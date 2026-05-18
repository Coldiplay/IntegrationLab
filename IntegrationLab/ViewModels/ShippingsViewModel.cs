using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using BaseLibrary.Model.Classes;
using BaseLibrary.Model.Enums;
using CommunityToolkit.Mvvm.Input;
using IntegrationLab.Model;
using IntegrationLab.Views;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationLab.ViewModels;

public partial class ShippingsViewModel : ViewModelControlBase<ShippingsView>
{
    public ObservableCollection<Shipping> Shippings
    {
        get;
        set
        {
            if (Equals(value, field)) return;
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentShippings));
            OnPropertyChanged(nameof(PastShippings));
        }
    } = [];

    public List<Shipping> CurrentShippings =>
        Shippings.Where(s => s.ShippingStatus is ShippingStatus.ReadyToShip
            or ShippingStatus.Shipping).ToList();

    public List<Shipping> PastShippings =>
        Shippings.Where(s => s.ShippingStatus == ShippingStatus.Delivered)
            .ToList();

    public ShippingsViewModel()
    {
        _client = App.Services.GetRequiredService<HttpClient>();
        Shippings = App.Services.GetRequiredService<HubData>().Shippings;
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
        //Shippings = _hub.InvokeAsync<object>("");
        Shippings = await _client.GetFromJsonAsync<ObservableCollection<Shipping>>("api/Shippings")
                    ?? [];
    }

    public void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Control control ||
            (control.Parent as ListBoxItem)?.Content is not Shipping shipping) return;
        OpenShippingCommand.Execute(shipping);
    }

    [RelayCommand]
    private static void OpenShipping(Shipping shipping)
    {
        var shippingView = App.Services.GetRequiredService<SingleShippingViewModel>();
        shippingView.Shipping = shipping;
        App.ChangeCurrentView(shippingView);
    }

    [RelayCommand]
    private void OpenActiveShipping()
    {
        var activeShipping = Shippings.FirstOrDefault(s => s.ShippingStatus == ShippingStatus.Shipping);
        if (activeShipping is not null)
            OpenShippingCommand.Execute(activeShipping);
    }
}