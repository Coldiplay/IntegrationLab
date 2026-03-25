using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BaseLibrary.Model;

[PrimaryKey(nameof(Id))]
public partial class Message : ObservableValidator
{
    public Guid Id { get; set; }
    
    [Required] [ObservableProperty] [MaxLength(300)] [MinLength(1)]
    private string _content = null!;
    [Required] [ObservableProperty] private DateTime _date;
    //public DateTime Date { get; set; }
    [Required] [ForeignKey(nameof(Sender))] public int SenderId { get; set; }
    [Required] [ForeignKey(nameof(Chat))] public int ChatId { get; set; }
    
    public virtual User Sender { get; set; }
    public virtual Chat Chat { get; set; }
}