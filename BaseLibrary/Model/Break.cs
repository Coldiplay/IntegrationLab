using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model;

[PrimaryKey(nameof(Id))]
public partial class Break : ObservableValidator
{
    public Guid Id { get; set; }

    [Required]
    [ObservableProperty]
    public partial BreakType BreakType { get; set; }

    [Required]
    [ObservableProperty]
    public partial DateTime Start { get; set; }

    [ObservableProperty]
    public partial DateTime? End { get; set; }
    [Required] public Guid ShiftId { get; set; }
    public virtual DriversShift Shift { get; set; }
}