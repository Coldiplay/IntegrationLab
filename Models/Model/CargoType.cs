using System.ComponentModel.DataAnnotations;

namespace Models.Model;

public partial class CargoType
{
    public int Id { get; set; }
    [Required] [MaxLength(60)] public string Title { get; set; } = null!;
}