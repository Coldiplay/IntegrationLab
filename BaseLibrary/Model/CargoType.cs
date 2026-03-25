using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Model;

public partial class CargoType
{
    public int Id { get; set; }
    [Required] [MaxLength(60)] public string Title { get; set; } = null!;
}