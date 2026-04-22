using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model;

[PrimaryKey(nameof(Id))]
[Index(nameof(Login),  IsUnique = true)]
public partial class User : ObservableValidator
{
    public int Id { get; set; }

    [Required]
    [ObservableProperty]
    [MaxLength(32)]
    public partial string Login { get; set; } = null!;

    [Required]
    [ObservableProperty]
    [MaxLength(40)]
    public partial string Name { get; set; } = null!;

    [Required]
    [ObservableProperty]
    [MaxLength(40)]
    public partial string LastName { get; set; } = null!;

    [ObservableProperty]
    [MaxLength(40)]
    public partial string Patronymic { get; set; } = null!;

    [Required]
    [ObservableProperty]
    public partial DateOnly HireDate { get; set; }

    [ObservableProperty]
    [MaxLength(20)]
    public partial string Phone { get; set; } = null!;

    [Required]
    [ObservableProperty]
    [MaxLength(512)]
    public partial string PassHash { get; set; } = null!;
}