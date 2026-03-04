using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Model;

[Keyless]
public partial class TransportCargoTypes
{
    [ForeignKey(nameof(Vehicle))] public int VehicleId { get; set; }
    [ForeignKey(nameof(CargoType))] public int CargoTypeId { get; set; }
    public virtual Vehicle Vehicle { get; set; }
    public virtual CargoType CargoType { get; set; }
}