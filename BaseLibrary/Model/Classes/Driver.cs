using System.ComponentModel.DataAnnotations.Schema;
using BaseLibrary.Model.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model.Classes;

[PrimaryKey(nameof(UserId))]
public partial class Driver : ObservableObject
{
    [ForeignKey(nameof(User))] public ulong UserId { get; set; }

    public Rights Rights { get; set; }

    public string? DriversLicense { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<DriversShift> DriversShifts { get; set; } = new List<DriversShift>();

    public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();

    public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();

    public virtual User User { get; set; } = null!;
}
