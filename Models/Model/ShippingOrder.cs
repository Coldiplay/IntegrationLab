using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Models.Model;

[PrimaryKey(nameof(Id))]
public partial class ShippingOrder : ObservableValidator
{
    public int Id { get; set; }

    [ObservableProperty] private DateTime _orderDate;
    [ObservableProperty] private DateTime _shippingDate;
    [ObservableProperty] private DateTime _sentDate;
    [ObservableProperty] private DateTime _receivedDate;

    public virtual List<Cargo> Cargos { get; set; } = [];
    
    //public Shipping? Shipping { get; set; }
    [MaxLength(120)]
    [ObservableProperty]
    private string _address = null!;

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