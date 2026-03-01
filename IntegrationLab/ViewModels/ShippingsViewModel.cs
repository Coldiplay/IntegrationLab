using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
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
    }

    private async Task ConfirmShipping(Shipping shipping)
    {
        //????
        new Thread(() =>
        {
            shipping.Confirmed = true;
            _client.PatchAsJsonAsync("api/Shippings/Confirm", shipping.Id);
        }).Start();
    }

    private async Task LoadShippings()
    {
        Shippings = await _client.GetFromJsonAsync<ObservableCollection<Shipping>>("api/Shippings")
            ?? [];
    }
}