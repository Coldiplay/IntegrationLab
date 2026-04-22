using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model;

[Keyless]
[Index(nameof(UserId), IsUnique = true)]
public partial class Driver : ObservableValidator
{
    [Required] [ForeignKey(nameof(User))] public int UserId { get; set; }
    [Required]
    [ObservableProperty]
    public partial Rights Rights { get; set; }

    [ObservableProperty]
    [MaxLength(40)]
    public partial string? DriversLicense { get; set; } = null!;
    public virtual User User { get; set; }
}