using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Models.Model;

[PrimaryKey(nameof(Id))]
public partial class Break : ObservableValidator
{
    public Guid Id { get; set; }

    [Required] [ObservableProperty] private BreakType _breakType;
    [Required] [ObservableProperty] private DateTime _start;
    [ObservableProperty] private DateTime? _end;
    [Required] public Guid ShiftId { get; set; }
    public virtual DriversShift Shift { get; set; }
}