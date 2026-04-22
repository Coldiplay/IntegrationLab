using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model;

[PrimaryKey(nameof(Id))]
public partial class ShippingOrder : ObservableValidator
{
    public int Id { get; set; }

    [Required]
    [ObservableProperty]
    public partial DateTime OrderDate { get; set; }

    [Required]
    [ObservableProperty]
    public partial string ReceiverFio { get; set; } = null!;

    [Required]
    [ObservableProperty]
    [MaxLength(120)]
    public partial string Address { get; set; } = null!;

    [Required]
    [ObservableProperty]
    public partial OrderStatus Status { get; set; }

    [Required]
    [ObservableProperty]
    public partial DateTime ShippingDate { get; set; }

    [ObservableProperty]
    public partial DateTime? SentDate { get; set; }

    [ObservableProperty]
    public partial DateTime? ReceivedDate { get; set; }

    // TODO: Добавить маршрут

    public virtual List<Cargo> Cargos { get; set; } = [];
    
    //public Shipping? Shipping { get; set; }

    public void AddCargo(Cargo cargo)
    {
        Cargos.Add(cargo);
        cargo.ShippingOrder = this;
    }
    public void AddRangeCargo(List<Cargo> cargos)
    {
        Cargos.AddRange(cargos);
        foreach (var cargo in cargos)
        {
            cargo.ShippingOrder = this;
        }
    }
    public void RemoveCargo(Cargo cargo)
    {
        cargo.ShippingOrder = null;
        Cargos.Remove(cargo);
        //GC.Collect();
    }
}