using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using BaseLibrary.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model;

[PrimaryKey(nameof(Id))]
public partial class Chat : ObservableValidator
{
    public int Id { get; set; }
    [MaxLength(60)] public string? Name { get; set; }
    public bool IsPrivateChat { get; set; } = true;

    public virtual ObservableCollection<Message> Messages
    {
        get;
        set
        {
            if (Equals(value, field)) return;
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(LastMessage));
            OnPropertyChanged(nameof(LastMessageText));
        }
    } = [];

    public Message? LastMessage => Messages.MaxBy(m => m.Date);

    public string? LastMessageText
    {
        get
        {
            var text = LastMessage?.Content.TruncateByWordsEfficient(20);
            return !string.IsNullOrEmpty(text) && text.Length < 17 ? text : text + "...";
        }
    }
}