using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model;

[PrimaryKey(nameof(Id))]
public partial class DriversShift : ObservableValidator
{
    public Guid Id { get; set; }
    [Required]
    [ObservableProperty]
    public partial DateTime StartDate { get; set; }

    [ObservableProperty]
    public partial DateTime? EndDate { get; set; }
    [Required] [ForeignKey(nameof(Driver))] public int DriverId { get; set; }
    
    public virtual User Driver { get; set; }
}