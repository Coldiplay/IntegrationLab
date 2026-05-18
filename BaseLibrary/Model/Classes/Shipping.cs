using System.ComponentModel.DataAnnotations.Schema;
using BaseLibrary.Model.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model.Classes;

[PrimaryKey(nameof(Id))]
public partial class Shipping : ObservableObject
{
    public ulong Id { get; set; }

    [ObservableProperty] public partial string DeliveryPoint { get; set; } = null!;

    [ObservableProperty] public partial DateTime EstimatedDeliveryDate { get; set; }

    [ObservableProperty] public partial DateTime? DeliveryDate { get; set; }

    [ObservableProperty] public partial ShippingStatus ShippingStatus { get; set; }

    [ObservableProperty] public partial DateOnly ShippingDate { get; set; }

    [ObservableProperty] public partial DateTime? ShippedDate { get; set; }

    [ObservableProperty, ForeignKey(nameof(Vehicle))] public partial ulong VehicleId { get; set; }

    [ForeignKey(nameof(DesignatedDriver))] public ulong DesignatedDriverId { get; set; }

    [ObservableProperty, NotifyPropertyChangedFor(nameof(CargoWeight))] 
    public virtual partial ICollection<Cargo> Cargos { get; set; } = new List<Cargo>();

    public virtual Driver DesignatedDriver { get; set; } = null!;

    [ObservableProperty] public virtual partial ICollection<Incident> Incidents { get; set; } = new List<Incident>();

    [ObservableProperty] public virtual partial Vehicle Vehicle { get; set; } = null!;

    [NotMapped] public double CargoWeight => Cargos.Sum(c => c.Weight) / 1000;

    [NotMapped, ObservableProperty] public partial string ConfirmedStatus { get; set; } = "Не подтверждён";
}
