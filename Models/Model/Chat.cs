using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Model;

[PrimaryKey(nameof(Id))]
public partial class Chat
{
    public int Id { get; set; }
    [MaxLength(60)] public string? Name { get; set; }
    public bool IsPrivateChat { get; set; } = true;

    // [ForeignKey(nameof(Receiver))]
    // public int ReceiverId { get; set; }
    // [ForeignKey(nameof(Sender))]
    // public int SenderId { get; set; }
    //
    // public virtual User Receiver { get; set; }
    // public virtual User Sender { get; set; }
}