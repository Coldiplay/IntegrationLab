using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Models.Model;

[PrimaryKey(nameof(Id))]
public partial class Vehicle : ObservableValidator
{
    public int Id { get; set; }
    //[ObservableProperty]
    //private double _engineVolume;
    //[ObservableProperty]
    //private int _enginePower;

    [ObservableProperty]
    [MaxLength(15)]
    private string _number = null!;
    [ObservableProperty] 
    [MaxLength(20)] 
    private string _brandName = null!;
    [ObservableProperty] 
    [MaxLength(40)] 
    private string _model = null!;

    [ObservableProperty] private float _liftingCapacity;
    [ObservableProperty] private VehicleBodyType _bodyType;
    [ObservableProperty] private Dimensions _vehicleSize;
    [ObservableProperty] private float _maxCargoVolume;
    [ObservableProperty] private VehicleFuelType _fuelType;
    [ObservableProperty] private float _maxFuelVolume;
    [ObservableProperty] private float _vehicleWeight;
    [ObservableProperty] private byte _numberOfAxes;

    //[ForeignKey(nameof(TransportCargoTypes))] public int TransportCargoTypesId { get; set; }
    public virtual List<TransportCargoTypes> TransportCargoTypes { get; set; }
}