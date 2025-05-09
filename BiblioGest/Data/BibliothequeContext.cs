using Microsoft.EntityFrameworkCore;
using BiblioGest.Models;

namespace BiblioGest.Data;

public class BibliothequeContext : DbContext
{
    public BibliothequeContext(DbContextOptions<BibliothequeContext> options)
        : base(options)
        {
        }

        public DbSet<Livre> Livres { get; set; } = null!;
        public DbSet<Adherent> Adherents { get; set; } = null!;
        public DbSet<Emprunt> Emprunts { get; set; } = null!;
        public DbSet<Categorie> Categories { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration de la relation Livre - Catégorie
            modelBuilder.Entity<Livre>()
                .HasOne(l => l.Categorie)
                .WithMany(c => c.Livres)
                .HasForeignKey(l => l.CategorieId)
                .OnDelete(DeleteBehavior.SetNull); // Si une catégorie est supprimée, les livres conservent une catégorie NULL

            // Configuration de la relation Livre - Emprunt
            modelBuilder.Entity<Emprunt>()
                .HasOne(e => e.Livre)
                .WithMany(l => l.Emprunts)
                .HasForeignKey(e => e.LivreId)
                .OnDelete(DeleteBehavior.Restrict); // Empêche la suppression d'un livre s'il est référencé dans des emprunts

            // Configuration de la relation Adherent - Emprunt
            modelBuilder.Entity<Emprunt>()
                .HasOne(e => e.Adherent)
                .WithMany(a => a.Emprunts)
                .HasForeignKey(e => e.AdherentId)
                .OnDelete(DeleteBehavior.Restrict); // Empêche la suppression d'un adhérent s'il a des emprunts

            // Index pour recherche rapide par ISBN
            modelBuilder.Entity<Livre>()
                .HasIndex(l => l.ISBN)
                .IsUnique();

            // Index pour recherche rapide par nom et prénom d'adhérent
            modelBuilder.Entity<Adherent>()
                .HasIndex(a => new { a.Nom, a.Prenom });

            // Configuration de l'entité User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Nom).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Prenom).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.DateCreation).IsRequired();
                
                // Add unique constraint for username
                entity.HasIndex(e => e.Username).IsUnique();
                // Add unique constraint for email
                entity.HasIndex(e => e.Email).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
} 