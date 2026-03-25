using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model;

[PrimaryKey(nameof(Id))]
public partial class ShippingOrder : ObservableValidator
{
    public int Id { get; set; }

    [Required] [ObservableProperty] private DateTime _orderDate;
    [Required] [ObservableProperty] private string _receiverFio = null!;
    [Required] [ObservableProperty] [MaxLength(120)] private string _address = null!;
    [Required] [ObservableProperty] private OrderStatus _status;
    [Required] [ObservableProperty] private DateTime _shippingDate;
    [ObservableProperty] private DateTime? _sentDate;
    [ObservableProperty] private DateTime? _receivedDate;
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