using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioGest.Models;

public class Adherent
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Nom { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Prenom { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [Phone]
    [StringLength(20)]
    public string Telephone { get; set; } = string.Empty;
    
    public DateTime DateInscription { get; set; } = DateTime.Now;
    
    // public string Statut { get; set; } = "Actif";
    
    // Navigation property pour les emprunts
    public virtual ICollection<Emprunt> Emprunts { get; set; } = new List<Emprunt>();
    
    
    [NotMapped]
    public string NomComplet => $"{Prenom} {Nom}";
} 