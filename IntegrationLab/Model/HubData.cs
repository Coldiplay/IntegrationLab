using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BaseLibrary.Model.Classes;
using BaseLibrary.Model.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace IntegrationLab.Model;

public partial class HubData : ObservableObject
{
    public Shipping? ActiveShipping => Shippings.FirstOrDefault(s => s.ShippingStatus == ShippingStatus.Shipping);

    [ObservableProperty]
    public partial ConcurrentDictionary<Chat, (ObservableCollection<User> members, ObservableCollection<Message> messages)> Chats { get; set; } = [];

    [ObservableProperty, NotifyParentProperty(true)] public partial ObservableCollection<Shipping> Shippings { get; set; } = [];
    [ObservableProperty] public partial ObservableCollection<Incident> Incidents { get; set; } = [];

    [ObservableProperty, NotifyPropertyChangedFor(nameof(Breaks))] public partial ObservableCollection<DriversShift> Shifts { get; set; } = [];
    public ObservableCollection<ShiftBreak> Breaks => [.. Shifts.SelectMany(s => s.ShiftBreaks)];
}