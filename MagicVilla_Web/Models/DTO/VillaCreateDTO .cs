using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Web.Models.DTO;

public class VillaCreateDTO
{
    [Required]
    [MaxLength(20)]
    public string Name { get; set; }
    [Required]
    public int Sqft { get; set; }
    [Required]
    public int Occupancy { get; set; }
    public string ImageUrl { get; set; }
    public string Amenity { get; set; }
    public string Details { get; set; }
    [Required]
    public double Rate { get; set; }
}
