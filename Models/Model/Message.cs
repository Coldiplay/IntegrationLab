using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Models.Model;

[PrimaryKey(nameof(Id))]
public partial class Message : ObservableValidator
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(300)]
    [MinLength(1)]
    [ObservableProperty]
    private string _content = null!;
    public DateTime Date { get; set; }
    [ForeignKey(nameof(Sender))]
    public int SenderId { get; set; }
    [ForeignKey(nameof(Chat))]
    public int ChatId { get; set; }
    
    public virtual User Sender { get; set; }
    public virtual Chat Chat { get; set; }
}