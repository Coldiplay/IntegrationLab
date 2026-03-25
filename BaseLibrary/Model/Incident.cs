using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model;

[PrimaryKey(nameof(Id))]
public partial class Incident : ObservableValidator
{
    public int Id { get; set; }

    [Required] [ForeignKey(nameof(Shipping))] public Guid ShippingId { get; set; }
    [Required] [ForeignKey(nameof(Driver))] public int DriverId { get; set; }
    [Required] [ObservableProperty] [MaxLength(500)] private string _description = null!;
    [Required] [ObservableProperty] private DateTime _incidentDate;
    [Required] [ObservableProperty] private IncidentStatus _status;
    
    public virtual Shipping Shipping { get; set; }
    public virtual User Driver { get; set; }
}