using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model;

[PrimaryKey(nameof(Id))]
public partial class Cargo : ObservableValidator
{
    public Guid Id { get; set; }

    [Required]
    [ObservableProperty]
    [MaxLength(40)]
    public partial string Name { get; set; } = null!;

    [ObservableProperty]
    [MaxLength(200)]
    public partial string? Description { get; set; }

    [Required]
    [ObservableProperty]
    public partial DateTime DateAdded { get; set; } = DateTime.Now;

    [Required]
    [ObservableProperty]
    public partial double Weight { get; set; }

    [Required]
    [ObservableProperty]
    public partial Dimensions Dimensions { get; set; }

    [ObservableProperty]
    public partial DangerLevel? DangerLevel { get; set; }
    [Required] [ForeignKey(nameof(ShippingOrder))] public int ShippingOrderId { get; set; }
    [ForeignKey(nameof(Shipping))] public Guid? ShippingId { get; set; }
    [Required] [ForeignKey(nameof(CargoType))] public int CargoTypeId { get; set; }
    
    
    public virtual CargoType CargoType { get; set; }
    //ВОзможно надо доббавить OnPropertyChanged
    public virtual ShippingOrder ShippingOrder { get; set; } = null!;
    public virtual Shipping? Shipping { get; set; }
}