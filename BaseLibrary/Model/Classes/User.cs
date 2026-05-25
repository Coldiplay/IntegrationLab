using BaseLibrary.Model.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model.Classes;

[PrimaryKey(nameof(Id))]
public partial class User : ObservableObject
{
    public Guid Id { get; set; }

    [ObservableProperty] public partial string Login { get; set; } = null!;

    [ObservableProperty] public partial string FirstName { get; set; } = null!;

    [ObservableProperty] public partial string LastName { get; set; } = null!;

    [ObservableProperty] public partial string? Patronymic { get; set; }

    [ObservableProperty] public partial DateOnly HireDate { get; set; }

    [ObservableProperty] public partial string Phone { get; set; } = null!;

    [ObservableProperty] public partial string Email { get; set; } = null!;

    [ObservableProperty] public partial Role Role { get; set; }

    [ObservableProperty] public virtual partial ICollection<Chat> Chats { get; set; } = new List<Chat>();

    [ObservableProperty] public virtual partial Driver? Driver { get; set; }

    [ObservableProperty] public virtual partial ICollection<Message> Messages { get; set; } = new List<Message>();
}
