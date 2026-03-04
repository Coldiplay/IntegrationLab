using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Models.Model;

public partial class Shipping : ObservableValidator
{
    public Guid Id { get; set; }

    [ObservableProperty]
    private DateTime _shippedDate;
    [ObservableProperty]
    private DateTime _estimatedDeliveryDate;

    [ObservableProperty] private DateTime? _deliveryDate;
    //Скорее всего не надо
    [ObservableProperty] private decimal _estimatedDeliveryPrice;

    [ObservableProperty] private decimal _estimatedDeliveryCost;
    //
    
    [ForeignKey(nameof(DesignatedDriver))] public int? DesignatedDriverId { get; set; }
    public virtual Driver? DesignatedDriver { get; set; }
    
    [ObservableProperty] private bool _confirmed = false;

    [NotMapped]
    public string ConfirmedStatus
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