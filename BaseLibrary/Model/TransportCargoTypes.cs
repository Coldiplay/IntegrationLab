using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model;

[Keyless]
public partial class TransportCargoTypes
{
    [Required] [ForeignKey(nameof(Vehicle))] public int VehicleId { get; set; }
    [Required] [ForeignKey(nameof(CargoType))] public int CargoTypeId { get; set; }
    public virtual Vehicle Vehicle { get; set; }
    public virtual CargoType CargoType { get; set; }
}