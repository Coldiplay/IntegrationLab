using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Models.Model;

public partial class User : ObservableValidator
{
    public int Id { get; set; }

    [ObservableProperty] 
    [MaxLength(32)] 
    private string _login = null!;
    
    [ObservableProperty]
    [MaxLength(40)]
    private string _name = null!;
    
    [ObservableProperty]
    [MaxLength(40)]
    private string _lastName = null!;
    
    [ObservableProperty]
    [MaxLength(20)]
    private string _phone = null!;

    [ObservableProperty]
    [MaxLength(512)]
    private string _passHash = null!;
}