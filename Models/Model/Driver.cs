using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Models.Model;

[Keyless]
[Index(nameof(UserId), IsUnique = true)]
public partial class Driver : ObservableValidator
{
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public virtual User User { get; set; }
    
    [ObservableProperty] private Rights _rights;
}