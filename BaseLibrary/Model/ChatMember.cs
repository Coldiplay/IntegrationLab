using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model;

[Keyless]
public partial class ChatMember
{
    [Required] [ForeignKey(nameof(Chat))] public int ChatId { get; set; }
    [Required] [ForeignKey(nameof(Member))] public int MemberId { get; set; }
    
    public virtual User Member { get; set; }
    public virtual Chat Chat { get; set; }
}