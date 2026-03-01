using CommunityToolkit.Mvvm.ComponentModel;

namespace Models.Model;

public partial class User : ObservableObject
{
    public int Id { get; set; }
    
    [ObservableProperty]
    private string _name = null!;

    [ObservableProperty] private string _lastName = null!;
    [ObservableProperty] private string _phone = null!;
}