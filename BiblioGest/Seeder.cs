using System;
using System.Linq;
using BiblioGest.Data;
using BiblioGest.Models;

namespace BiblioGest
{
    public static class Seeder
    {
        public static void Seed(BibliothequeContext context)
        {
            // Catégories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Categorie { Nom = "Roman", Description = "Romans et fictions" },
                    new Categorie { Nom = "Science", Description = "Livres scientifiques" },
                    new Categorie { Nom = "Histoire", Description = "Ouvrages historiques" },
                    new Categorie { Nom = "Jeunesse", Description = "Livres pour enfants" }
                );
                context.SaveChanges();
            }

            // Adhérents
            if (!context.Adherents.Any())
            {
                context.Adherents.AddRange(
                    new Adherent { Nom = "Dupont", Prenom = "Alice", Email = "alice.dupont@mail.com", Telephone = "0600000001", DateInscription = DateTime.Now.AddDays(-30) },
                    new Adherent { Nom = "Martin", Prenom = "Bob", Email = "bob.martin@mail.com", Telephone = "0600000002", DateInscription = DateTime.Now.AddDays(-10) },
                    new Adherent { Nom = "Durand", Prenom = "Claire", Email = "claire.durand@mail.com", Telephone = "0600000003", DateInscription = DateTime.Now.AddDays(-5) }
                );
                context.SaveChanges();
            }

            // Livres
            if (!context.Livres.Any())
            {
                var catRoman = context.Categories.FirstOrDefault(c => c.Nom == "Roman");
                var catScience = context.Categories.FirstOrDefault(c => c.Nom == "Science");
                var catHistoire = context.Categories.FirstOrDefault(c => c.Nom == "Histoire");
                var catJeunesse = context.Categories.FirstOrDefault(c => c.Nom == "Jeunesse");
                context.Livres.AddRange(
                    new Livre { ISBN = "9781234567890", Titre = "Le Grand Voyage", Auteur = "Jean Valjean", Editeur = "Gallimard", AnneePublication = 2015, Disponible = true, Categorie = catRoman },
                    new Livre { ISBN = "9780987654321", Titre = "La Science Facile", Auteur = "Marie Curie", Editeur = "Dunod", AnneePublication = 2018, Disponible = true, Categorie = catScience },
                    new Livre { ISBN = "9781111111111", Titre = "Histoire de France", Auteur = "Paul Dubois", Editeur = "Larousse", AnneePublication = 2012, Disponible = true, Categorie = catHistoire },
                    new Livre { ISBN = "9782222222222", Titre = "Contes pour enfants", Auteur = "Sophie Martin", Editeur = "Nathan", AnneePublication = 2020, Disponible = true, Categorie = catJeunesse }
                );
                context.SaveChanges();
            }

            // Emprunts
            if (!context.Emprunts.Any())
            {
                var livre1 = context.Livres.FirstOrDefault();
                var adherent1 = context.Adherents.FirstOrDefault();
                if (livre1 != null && adherent1 != null)
                {
                    context.Emprunts.Add(new Emprunt
                    {
                        LivreId = livre1.Id,
                        AdherentId = adherent1.Id,
                        DateEmprunt = DateTime.Now.AddDays(-3),
                        DateRetourPrevue = DateTime.Now.AddDays(7),
                        DateRetour = null
                    });
                    livre1.Disponible = false;
                    context.SaveChanges();
                }
            }

            // Catégories supplémentaires
            if (context.Categories.Count() < 14)
            {
                context.Categories.AddRange(
                    new Categorie { Nom = "Philosophie", Description = "Ouvrages philosophiques" },
                    new Categorie { Nom = "Poésie", Description = "Recueils de poèmes" },
                    new Categorie { Nom = "Biographie", Description = "Vies de personnages célèbres" },
                    new Categorie { Nom = "Cuisine", Description = "Livres de recettes" },
                    new Categorie { Nom = "Voyage", Description = "Guides et récits de voyage" },
                    new Categorie { Nom = "Art", Description = "Livres d'art et d'artistes" },
                    new Categorie { Nom = "Informatique", Description = "Programmation et nouvelles technologies" },
                    new Categorie { Nom = "Santé", Description = "Bien-être et médecine" },
                    new Categorie { Nom = "Économie", Description = "Ouvrages économiques" },
                    new Categorie { Nom = "Théâtre", Description = "Pièces et analyses théâtrales" }
                );
                context.SaveChanges();
            }

            // Adhérents supplémentaires
            if (context.Adherents.Count() < 13)
            {
                context.Adherents.AddRange(
                    new Adherent { Nom = "Petit", Prenom = "Lucas", Email = "lucas.petit@mail.com", Telephone = "0600000004", DateInscription = DateTime.Now.AddDays(-20) },
                    new Adherent { Nom = "Leroy", Prenom = "Emma", Email = "emma.leroy@mail.com", Telephone = "0600000005", DateInscription = DateTime.Now.AddDays(-15) },
                    new Adherent { Nom = "Moreau", Prenom = "Hugo", Email = "hugo.moreau@mail.com", Telephone = "0600000006", DateInscription = DateTime.Now.AddDays(-12) },
                    new Adherent { Nom = "Simon", Prenom = "Léa", Email = "lea.simon@mail.com", Telephone = "0600000007", DateInscription = DateTime.Now.AddDays(-8) },
                    new Adherent { Nom = "Laurent", Prenom = "Noah", Email = "noah.laurent@mail.com", Telephone = "0600000008", DateInscription = DateTime.Now.AddDays(-6) },
                    new Adherent { Nom = "Garcia", Prenom = "Chloé", Email = "chloe.garcia@mail.com", Telephone = "0600000009", DateInscription = DateTime.Now.AddDays(-4) },
                    new Adherent { Nom = "Roux", Prenom = "Louis", Email = "louis.roux@mail.com", Telephone = "0600000010", DateInscription = DateTime.Now.AddDays(-3) },
                    new Adherent { Nom = "Fournier", Prenom = "Jade", Email = "jade.fournier@mail.com", Telephone = "0600000011", DateInscription = DateTime.Now.AddDays(-2) },
                    new Adherent { Nom = "Girard", Prenom = "Gabriel", Email = "gabriel.girard@mail.com", Telephone = "0600000012", DateInscription = DateTime.Now.AddDays(-1) },
                    new Adherent { Nom = "Andre", Prenom = "Manon", Email = "manon.andre@mail.com", Telephone = "0600000013", DateInscription = DateTime.Now }
                );
                context.SaveChanges();
            }

            // Livres supplémentaires
            if (context.Livres.Count() < 14)
            {
                var categories = context.Categories.ToList();
                context.Livres.AddRange(
                    new Livre { ISBN = "9783333333333", Titre = "Philosophie Moderne", Auteur = "René Descartes", Editeur = "PUF", AnneePublication = 2010, Disponible = true, Categorie = categories.FirstOrDefault(c => c.Nom == "Philosophie") },
                    new Livre { ISBN = "9784444444444", Titre = "Poèmes du Soir", Auteur = "Paul Verlaine", Editeur = "Seuil", AnneePublication = 2005, Disponible = true, Categorie = categories.FirstOrDefault(c => c.Nom == "Poésie") },
                    new Livre { ISBN = "9785555555555", Titre = "Steve Jobs", Auteur = "Walter Isaacson", Editeur = "Fayard", AnneePublication = 2011, Disponible = true, Categorie = categories.FirstOrDefault(c => c.Nom == "Biographie") },
                    new Livre { ISBN = "9786666666666", Titre = "Cuisine Facile", Auteur = "Julie Andrieu", Editeur = "Hachette", AnneePublication = 2019, Disponible = true, Categorie = categories.FirstOrDefault(c => c.Nom == "Cuisine") },
                    new Livre { ISBN = "9787777777777", Titre = "Voyage en Italie", Auteur = "Marco Polo", Editeur = "Michelin", AnneePublication = 2016, Disponible = true, Categorie = categories.FirstOrDefault(c => c.Nom == "Voyage") },
                    new Livre { ISBN = "9788888888888", Titre = "L'Art Moderne", Auteur = "Claude Monet", Editeur = "Flammarion", AnneePublication = 2013, Disponible = true, Categorie = categories.FirstOrDefault(c => c.Nom == "Art") },
                    new Livre { ISBN = "9789999999999", Titre = "C# pour les Nuls", Auteur = "John Sharp", Editeur = "First Interactive", AnneePublication = 2021, Disponible = true, Categorie = categories.FirstOrDefault(c => c.Nom == "Informatique") },
                    new Livre { ISBN = "9781010101010", Titre = "Bien-être au quotidien", Auteur = "Michel Cymes", Editeur = "Marabout", AnneePublication = 2017, Disponible = true, Categorie = categories.FirstOrDefault(c => c.Nom == "Santé") },
                    new Livre { ISBN = "9781212121212", Titre = "Comprendre l'économie", Auteur = "Thomas Piketty", Editeur = "Seuil", AnneePublication = 2014, Disponible = true, Categorie = categories.FirstOrDefault(c => c.Nom == "Économie") },
                    new Livre { ISBN = "9781313131313", Titre = "Le Cid", Auteur = "Corneille", Editeur = "GF Flammarion", AnneePublication = 2002, Disponible = true, Categorie = categories.FirstOrDefault(c => c.Nom == "Théâtre") }
                );
                context.SaveChanges();
            }

            // Emprunts supplémentaires
            if (context.Emprunts.Count() < 11)
            {
                var livres = context.Livres.Take(10).ToList();
                var adherents = context.Adherents.Take(10).ToList();
                for (int i = 0; i < 10; i++)
                {
                    var livre = livres[i];
                    var adherent = adherents[i];
                    context.Emprunts.Add(new Emprunt
                    {
                        LivreId = livre.Id,
                        AdherentId = adherent.Id,
                        DateEmprunt = DateTime.Now.AddDays(-i),
                        DateRetourPrevue = DateTime.Now.AddDays(7 - i),
                        DateRetour = i % 2 == 0 ? (DateTime?)DateTime.Now.AddDays(-i + 2) : null
                    });
                    livre.Disponible = i % 2 != 0; // Un livre sur deux est encore emprunté
                }
                context.SaveChanges();
            }
        }
    }
} 