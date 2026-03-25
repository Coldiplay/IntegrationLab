using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model;

[PrimaryKey(nameof(Id))]
[Index(nameof(Login),  IsUnique = true)]
public partial class User : ObservableValidator
{
    public int Id { get; set; }

    [Required] [ObservableProperty] [MaxLength(32)] private string _login = null!;
    [Required] [ObservableProperty] [MaxLength(40)] private string _name = null!;
    [Required] [ObservableProperty] [MaxLength(40)] private string _lastName = null!;
    [ObservableProperty] [MaxLength(40)] private string _patronymic = null!;
    [Required] [ObservableProperty] private DateOnly _hireDate;
    [ObservableProperty] [MaxLength(20)] private string _phone = null!;
    [Required] [ObservableProperty] [MaxLength(512)] private string _passHash = null!;
}