using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model.Classes;

[PrimaryKey(nameof(Id))]
public partial class Message : ObservableValidator
{
    public ulong Id { get; set; }

    [ObservableProperty, Required, MaxLength(300), MinLength(1)] 
    public partial string Content { get; set; } = null!;

    [ForeignKey(nameof(Sender))] public ulong SenderId { get; set; }

    [ForeignKey(nameof(Chat))] public ulong ChatId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Chat Chat { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
