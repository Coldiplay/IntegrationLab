using System.ComponentModel.DataAnnotations.Schema;
using BaseLibrary.Model.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BaseLibrary.Model.Classes;

public partial class Vehicle : ObservableObject
{
    public ulong Id { get; set; }

    [ObservableProperty] public partial string VehicleNumberPlate { get; set; } = null!;

    [ObservableProperty, NotifyPropertyChangedFor(nameof(VehicleFullName))] 
    public partial string Brand { get; set; } = null!;

    [ObservableProperty, NotifyPropertyChangedFor(nameof(VehicleFullName))] 
    public partial string Model { get; set; } = null!;

    [NotMapped] public string VehicleFullName => $"{Brand} {Model}";

    [ObservableProperty] public partial Rights NeededRights { get; set; }

    [ObservableProperty] public partial double LiftingCapacity { get; set; }

    [ObservableProperty] public partial BodyType BodyType { get; set; }
    [ObservableProperty] public partial Dimensions VehicleSize { get; set; }

    [ObservableProperty] public partial Dimensions BodySize { get; set; }

    [ObservableProperty] public partial double MaxCargoVolume { get; set; }

    [ObservableProperty] public partial double VehicleWeight { get; set; }

    public byte NumberOfAxes { get; set; }

    [ObservableProperty] public virtual partial ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();

    [ObservableProperty] public virtual partial ICollection<CargoType> SupportedCargoTypes { get; set; } = new List<CargoType>();
}
