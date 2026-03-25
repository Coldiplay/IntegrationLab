using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model;

[PrimaryKey(nameof(Id))]
public partial class Cargo : ObservableValidator
{
    public Guid Id { get; set; }
    
    [Required] [ObservableProperty] [MaxLength(40)] private string _name = null!;
    [ObservableProperty] [MaxLength(200)] private string? _description;
    [Required] [ObservableProperty] private DateTime _dateAdded = DateTime.Now;
    [Required] [ObservableProperty] private double _weight;
    [Required] [ObservableProperty] private Dimensions _dimensions;
    [ObservableProperty] private DangerLevel? _dangerLevel;
    
    [Required] [ForeignKey(nameof(ShippingOrder))] public int ShippingOrderId { get; set; }
    [ForeignKey(nameof(Shipping))] public Guid? ShippingId { get; set; }
    [Required] [ForeignKey(nameof(CargoType))] public int CargoTypeId { get; set; }
    
    
    public virtual CargoType CargoType { get; set; }
    //ВОзможно надо доббавить OnPropertyChanged
    public virtual ShippingOrder ShippingOrder { get; set; } = null!;
    public virtual Shipping? Shipping { get; set; }
}