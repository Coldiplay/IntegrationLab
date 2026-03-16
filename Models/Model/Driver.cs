using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Models.Model;

[Keyless]
[Index(nameof(UserId), IsUnique = true)]
public partial class Driver : ObservableValidator
{
    [Required] [ForeignKey(nameof(User))] public int UserId { get; set; }
    [Required] [ObservableProperty] private Rights _rights;
    [ObservableProperty] [MaxLength(40)] private string? _driversLicense = null!;
    
    public virtual User User { get; set; }
}