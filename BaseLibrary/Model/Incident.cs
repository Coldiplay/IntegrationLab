using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model;

[PrimaryKey(nameof(Id))]
public partial class Incident : ObservableValidator
{
    public int Id { get; set; }

    [ForeignKey(nameof(Shipping))] public Guid ShippingId { get; set; }
    [Required] [ForeignKey(nameof(Driver))] public int DriverId { get; set; }
    [Required] [ObservableProperty] [MaxLength(500)] public partial string Description { get; set; } = null!;
    [Required] [ObservableProperty] public partial DateTime IncidentDate { get; set; }
    [Required] [ObservableProperty] public partial IncidentStatus Status { get; set; }
    public virtual Shipping? Shipping { get; set; }
    public virtual User Driver { get; set; }
}