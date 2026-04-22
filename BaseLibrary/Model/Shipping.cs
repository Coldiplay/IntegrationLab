using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BaseLibrary.Model;

public partial class Shipping : ObservableValidator
{
    public Guid Id { get; set; }
    [Required]
    [ObservableProperty]
    [MaxLength(120)]
    public partial string DeliveryPoint { get; set; }

    [ObservableProperty]
    public partial DateTime ShippedDate { get; set; }

    [ObservableProperty]
    public partial DateTime EstimatedDeliveryDate { get; set; }

    [ObservableProperty]
    public partial DateTime? DeliveryDate { get; set; }

    //[ObservableProperty] private bool _confirmed = false;
    [Required]
    [ObservableProperty]
    public partial ShippingStatus ShippingStatus { get; set; }

    //Скорее всего не надо
    [ObservableProperty] private decimal _estimatedDeliveryPrice;
    [ObservableProperty]
    public partial decimal EstimatedDeliveryCost { get; set; }

    //

    [Required] [ForeignKey(nameof(Vehicle))] public int VehicleId { get; set; }
    [ForeignKey(nameof(DesignatedDriver))] public int? DesignatedDriverId { get; set; }
    
    
    public virtual Vehicle Vehicle { get; set; }
    public virtual User? DesignatedDriver { get; set; }

    [NotMapped] public string ConfirmedStatus
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            OnPropertyChanged();
        }
    } = "Не подтверждён";

    public virtual List<Cargo> Cargos { get; set; } = [];

    public void AddCargo(Cargo cargo)
    {
        cargo.Shipping = this;
        Cargos.Add(cargo);
    }

    public void AddRangeCargo(List<Cargo> cargos)
    {
        Cargos.AddRange(cargos);
        foreach (var cargo in cargos)
        {
            cargo.Shipping = this;
        }
    }

    public void RemoveCargo(Cargo cargo)
    {
        Cargos.Remove(cargo);
        cargo.Shipping = null;
    }
}