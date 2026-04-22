using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model;

[PrimaryKey(nameof(Id))]
public partial class Vehicle : ObservableValidator
{
    public int Id { get; set; }
    //[ObservableProperty] private double _engineVolume;
    //[ObservableProperty] private int _enginePower;

    [Required]
    [ObservableProperty]
    [MaxLength(15)]
    public partial string Number { get; set; } = null!;

    [Required]
    [ObservableProperty]
    [MaxLength(20)]
    public partial string BrandName { get; set; } = null!;

    [Required]
    [ObservableProperty]
    [MaxLength(40)]
    public partial string Model { get; set; } = null!;

    [ObservableProperty]
    public partial Rights NeededRights { get; set; }

    [Required]
    [ObservableProperty]
    public partial float LiftingCapacity { get; set; }

    [Required]
    [ObservableProperty]
    public partial VehicleBodyType BodyType { get; set; }

    [Required]
    [ObservableProperty]
    public partial Dimensions VehicleSize { get; set; }

    [Required]
    [ObservableProperty]
    public partial Dimensions BodySize { get; set; }

    [Required]
    [ObservableProperty]
    public partial float MaxCargoVolume { get; set; }

    [Required]
    [ObservableProperty]
    public partial VehicleFuelType FuelType { get; set; }

    [Required]
    [ObservableProperty]
    public partial float MaxFuelVolume { get; set; }

    [Required]
    [ObservableProperty]
    public partial float VehicleWeight { get; set; }

    [ObservableProperty]
    public partial byte NumberOfAxes { get; set; }

    [NotMapped] public string VehicleFullName => $"{BrandName} {Model} {Number}";
    //[ForeignKey(nameof(TransportCargoTypes))] public int TransportCargoTypesId { get; set; }
    //public virtual List<TransportCargoTypes> TransportCargoTypes { get; set; }
}