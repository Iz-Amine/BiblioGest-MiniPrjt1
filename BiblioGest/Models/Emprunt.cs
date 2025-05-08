using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblioGest.Models;

public class Emprunt
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public DateTime DateEmprunt { get; set; } = DateTime.Now;
    
    public DateTime? DateRetour { get; set; }
    
    [Required]
    public DateTime DateRetourPrevue { get; set; }
    
    public bool EstEnRetard => !DateRetour.HasValue && DateTime.Now > DateRetourPrevue;
    
    [NotMapped] // Propriété calculée, pas stockée en DB
    public string Statut => DateRetour.HasValue 
        ? "Retourné" 
        : (EstEnRetard ? "En retard" : "En cours");
    
    // Relations
    public int LivreId { get; set; }
    
    [ForeignKey(nameof(LivreId))]
    public virtual Livre Livre { get; set; } = null!;
    
    public int AdherentId { get; set; }
    
    [ForeignKey(nameof(AdherentId))]
    public virtual Adherent Adherent { get; set; } = null!;
}