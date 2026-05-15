using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BaseLibrary.Model.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model.Classes;

[PrimaryKey(nameof(Id))]
public partial class Incident : ObservableValidator
{
    public ulong Id { get; set; }

    [ObservableProperty, ForeignKey(nameof(Shipping))] public partial ulong ShippingId { get; set; }

    [ObservableProperty, ForeignKey(nameof(Driver))] public partial ulong DriverId { get; set; }

    [ObservableProperty, Required, MaxLength(500), MinLength(8)] 
    public partial string Description { get; set; } = null!;

    [ObservableProperty, Required] public partial DateTime IncidentDate { get; set; }

    [ObservableProperty, Required] public partial IncidentStatus Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Driver Driver { get; set; } = null!;

    public virtual Shipping Shipping { get; set; } = null!;
}
