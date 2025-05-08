using System.ComponentModel.DataAnnotations;

namespace BiblioGest.Models;

public class Categorie
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Nom { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;
    
    // Navigation property pour les livres
    public virtual ICollection<Livre> Livres { get; set; } = new List<Livre>();
}