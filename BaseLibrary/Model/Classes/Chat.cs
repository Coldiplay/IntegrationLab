using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BaseLibrary.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model.Classes;

[PrimaryKey(nameof(Id))]
public partial class Chat : ObservableValidator
{
    public ulong Id { get; set; }

    [ObservableProperty, Required, MaxLength(60)] public partial string? Name { get; set; }

    [ObservableProperty, Required] public partial bool? IsPrivateChat { get; set; }

    [ObservableProperty] public partial DateTime? CreatedAt { get; set; }

    [ObservableProperty] public virtual partial ICollection<User> ChatMembers { get; set; } = new List<User>();

    [ObservableProperty, 
     NotifyPropertyChangedFor(nameof(LastMessage), [nameof(LastMessageText)])] 
    public virtual partial ICollection<Message> Messages { get; set; } = new List<Message>();

    [NotMapped] public Message? LastMessage => Messages.MaxBy(m => m.CreatedAt);

    [NotMapped] public string LastMessageText
    {
        get
        {
            var text = LastMessage?.Content.TruncateByWordsEfficient(20) 
                       ?? string.Empty;
            return text.Length < 17 ? text : text + "...";
        }
    }
}
