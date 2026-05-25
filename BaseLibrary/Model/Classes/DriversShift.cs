using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BaseLibrary.Model.Classes;

public partial class DriversShift : ObservableValidator
{
    public ulong Id { get; set; }

    [ObservableProperty, Required] public partial DateTime Start { get; set; }

    [ObservableProperty] public partial DateTime? End { get; set; }

    [ForeignKey(nameof(Driver))] public Guid DriverId { get; set; }

    public virtual Driver Driver { get; set; } = null!;

    [ObservableProperty] public virtual partial ICollection<ShiftBreak> ShiftBreaks { get; set; } = new List<ShiftBreak>();
}
