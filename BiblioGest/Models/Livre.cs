using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioGest.Models;

public class Livre
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(13)]
    public string ISBN { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    public string Titre { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Auteur { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string Editeur { get; set; } = string.Empty;
    
    public int AnneePublication { get; set; }
    
    public bool Disponible { get; set; } = true;
    
    // Navigation property pour les emprunts
    public virtual ICollection<Emprunt> Emprunts { get; set; } = new List<Emprunt>();
    
    
    public int? CategorieId { get; set; }

    [ForeignKey(nameof(CategorieId))]
    public virtual Categorie? Categorie { get; set; }
} 