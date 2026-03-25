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

    [Required] [ObservableProperty] [MaxLength(15)] private string _number = null!;
    [Required] [ObservableProperty] [MaxLength(20)] private string _brandName = null!;
    [Required] [ObservableProperty] [MaxLength(40)] private string _model = null!;
    [ObservableProperty] private Rights _neededRights;
    [Required] [ObservableProperty] private float _liftingCapacity;
    [Required] [ObservableProperty] private VehicleBodyType _bodyType;
    [Required] [ObservableProperty] private Dimensions _vehicleSize;
    [Required] [ObservableProperty] private Dimensions _bodySize;
    [Required] [ObservableProperty] private float _maxCargoVolume;
    [Required] [ObservableProperty] private VehicleFuelType _fuelType;
    [Required] [ObservableProperty] private float _maxFuelVolume;
    [Required] [ObservableProperty] private float _vehicleWeight;
    [ObservableProperty] private byte _numberOfAxes;


    [NotMapped] public string VehicleFullName => $"{BrandName} {Model} {Number}";
    //[ForeignKey(nameof(TransportCargoTypes))] public int TransportCargoTypesId { get; set; }
    //public virtual List<TransportCargoTypes> TransportCargoTypes { get; set; }
}