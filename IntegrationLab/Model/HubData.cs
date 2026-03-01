using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Models.Model;

namespace IntegrationLab.Model;

public partial class HubData : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Chat> _chats = [];
    [ObservableProperty]
    private ObservableCollection<Shipping> _shippings = [];
}