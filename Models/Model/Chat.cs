using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Model;

[PrimaryKey(nameof(Id))]
public partial class Chat
{
    public int Id { get; set; }
    [ForeignKey(nameof(Receiver))]
    public int ReceiverId { get; set; }
    [ForeignKey(nameof(Sender))]
    public int SenderId { get; set; }
    
    public virtual User Receiver { get; set; }
    public virtual User Sender { get; set; }
}