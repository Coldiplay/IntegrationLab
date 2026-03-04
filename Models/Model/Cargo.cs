using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Models.Model;

[PrimaryKey(nameof(Id))]
public partial class Cargo : ObservableValidator
{
    public Guid Id { get; set; }
    
    [MaxLength(40)]
    [ObservableProperty]
    private string _name = null!;
    [MaxLength(200)]
    [ObservableProperty]
    private string? _description;
    [ObservableProperty]
    private DateTime _dateAdded = DateTime.Now;
    [ObservableProperty]
    private double _weight;
    [ObservableProperty]
    private Dimensions _dimensions;
    [ObservableProperty]
    private DangerLevel _dangerLevel;
    
    [ForeignKey(nameof(ShippingOrder))]
    public int ShippingOrderId { get; set; }
    [ForeignKey(nameof(Shipping))]
    public Guid? ShippingId { get; set; }
    [ForeignKey(nameof(CargoType))]
    public int CargoTypeId { get; set; }
    public virtual CargoType CargoType { get; set; }

    //ВОзможно надо доббавить OnPropertyChanged
    public virtual ShippingOrder ShippingOrder { get; set; } = null!;
    public virtual Shipping? Shipping { get; set; }
}