using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model.Classes;

[PrimaryKey(nameof(Id))]
public partial class ShiftBreak : ObservableObject
{
    public ulong Id { get; set; }

    [ForeignKey(nameof(Shift))] public ulong ShiftId { get; set; }

    [ObservableProperty] public partial DateTime Start { get; set; }

    [ObservableProperty] public partial DateTime? End { get; set; }
    
    public virtual DriversShift Shift { get; set; } = null!;
}
