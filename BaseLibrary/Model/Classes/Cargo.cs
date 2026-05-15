using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model.Classes;

[PrimaryKey(nameof(Id))]
public partial class Cargo : ObservableObject
{
    public ulong Id { get; set; }

    [ObservableProperty] public partial string Name { get; set; } = null!;

    [ObservableProperty] public partial string? Description { get; set; }

    [ObservableProperty] public partial double Weight { get; set; }

    [ObservableProperty] public partial Dimensions Dimensions { get; set; }

    [ObservableProperty] public partial string? DangerLevel { get; set; }

    [ObservableProperty, ForeignKey(nameof(ShippingId))] 
    public partial ulong ShippingOrderId { get; set; }

    [ObservableProperty, ForeignKey(nameof(Shipping))] 
    public partial ulong? ShippingId { get; set; }

    [ObservableProperty, ForeignKey(nameof(CargoType))] 
    public partial ulong CargoTypeId { get; set; }

    [ObservableProperty] public virtual partial CargoType CargoType { get; set; } = null!;

    [ObservableProperty] public virtual partial Shipping? Shipping { get; set; }
}
