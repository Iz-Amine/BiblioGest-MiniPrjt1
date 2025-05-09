using System;
using System.Windows.Controls;
using System.Windows;
using System.Linq;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;

namespace BiblioGest.Pages
{
    public partial class StatistiquesPage : Page
    {
        public StatistiquesPage()
        {
            InitializeComponent();
            ChargerStatistiques();
            ChargerDernieresActivites();
        }

        private void ChargerStatistiques()
        {
            try
            {
                // Mise à jour de la date et l'heure
                DerniereMiseAJour.Text = $"Dernière mise à jour : {DateTime.Now:dd/MM/yyyy à HH:mm}";

                // Statistiques de base
                var totalLivres = App.DbContext.Livres.Count();
                var totalAdherents = App.DbContext.Adherents.Count();
                var totalEmprunts = App.DbContext.Emprunts.Count();
                // Calcul robuste : livres empruntés = livres avec un emprunt actif (DateRetour == null)
                var livresEmpruntes = App.DbContext.Emprunts
                    .Where(e => e.DateRetour == null)
                    .Select(e => e.LivreId)
                    .Distinct()
                    .Count();
                var livresDisponibles = totalLivres - livresEmpruntes;

                // Mise à jour des premières statistiques
                TotalLivres.Text = totalLivres.ToString("N0");
                TotalAdherents.Text = totalAdherents.ToString("N0");
                TotalEmprunts.Text = totalEmprunts.ToString("N0");
                LivresDisponibles.Text = livresDisponibles.ToString("N0");
                LivresEmpruntes.Text = livresEmpruntes.ToString("N0");

                // Calcul et mise à jour des pourcentages
                double pourcentageDisponibles = totalLivres > 0 ? (double)livresDisponibles / totalLivres * 100 : 0;
                double pourcentageEmpruntes = totalLivres > 0 ? (double)livresEmpruntes / totalLivres * 100 : 0;

                DisponiblesProgressBar.Value = pourcentageDisponibles;
                EmpruntesProgressBar.Value = pourcentageEmpruntes;

                PourcentageDisponibles.Text = $"{pourcentageDisponibles:F1}% du catalogue";
                PourcentageEmpruntes.Text = $"{pourcentageEmpruntes:F1}% du catalogue";

                // Statistiques détaillées
                var empruntsEnCoursList = App.DbContext.Emprunts
                    .Where(e => e.DateRetour == null)
                    .AsEnumerable();
                var empruntsEnCours = empruntsEnCoursList.Count(e => !e.EstEnRetard);
                var empruntsEnRetard = empruntsEnCoursList.Count(e => e.EstEnRetard);
                
                // Nouveaux adhérents dans les 30 derniers jours
                var il30Jours = DateTime.Now.AddDays(-30);
                var nouveauxAdherents = App.DbContext.Adherents.Count(a => a.DateInscription >= il30Jours);
                
                // Moyenne de livres par adhérent
                double livresParAdherent = totalAdherents > 0 ? (double)totalEmprunts / totalAdherents : 0;
                
                // Durée moyenne d'emprunt (pour les emprunts terminés)
                var empruntsTermines = App.DbContext.Emprunts
                    .Where(e => e.DateRetour.HasValue)
                    .ToList();
                
                double dureeEmpruntMoyenne = 0;
                if (empruntsTermines.Any())
                {
                    dureeEmpruntMoyenne = empruntsTermines.Average(e => 
                        (e.DateRetour.Value - e.DateEmprunt).TotalDays);
                }
                
                // Taux de rotation (nombre d'emprunts divisé par le nombre de livres)
                double tauxRotation = totalLivres > 0 ? (double)totalEmprunts / totalLivres * 100 : 0;
                
                // Mise à jour des statistiques détaillées
                EmpruntsEnCours.Text = $"Emprunts en cours : {empruntsEnCours}";
                EmpruntsEnRetard.Text = $"Emprunts en retard : {empruntsEnRetard}";
                NouveauxAdherents.Text = $"Nouveaux adhérents (30j) : {nouveauxAdherents}";
                LivresParAdherent.Text = $"Moyenne de livres par adhérent : {livresParAdherent:F1}";
                DureeEmpruntMoyenne.Text = $"Durée moyenne d'emprunt : {dureeEmpruntMoyenne:F1} jours";
                TauxRotation.Text = $"Taux de rotation : {tauxRotation:F1}%";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des statistiques : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ChargerStatistiques();
        }

        private void BtnNouvelEmprunt_Click(object sender, RoutedEventArgs e)
        {
            // Navigation vers la page des emprunts
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.MainFrame.Navigate(new Pages.EmpruntsPage());
        }

        private void BtnNouveauLivre_Click(object sender, RoutedEventArgs e)
        {
            // Navigation vers la page des livres
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.MainFrame.Navigate(new Pages.LivresPage());
        }

        private void BtnNouvelAdherent_Click(object sender, RoutedEventArgs e)
        {
            // Navigation vers la page des adhérents
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.MainFrame.Navigate(new Pages.AdherentsPage());
        }

        public class ActiviteRecente
        {
            public string Icone { get; set; } = "";
            public string Description { get; set; } = "";
            public DateTime Date { get; set; }
            
            // Propriété pour le regroupement par jour
            public string DateGroup => Date.Date.ToString("dd MMMM yyyy");
    
            // Heure formatée pour l'affichage
            public string HeureFormatee => Date.ToString("HH:mm");
        }

       private void ChargerDernieresActivites()
{
    // Afficher les 10 dernières activités (livres ajoutés, emprunts, retours, nouveaux adhérents)
    var activites = new System.Collections.Generic.List<ActiviteRecente>();

    // Derniers livres ajoutés
    var livres = App.DbContext.Livres
        .OrderByDescending(l => l.Id)
        .Take(3)
        .ToList()
        .Select(l => new ActiviteRecente {
            Icone = "📚",
            Description = $"Livre ajouté : {l.Titre} ({l.Auteur})",
            Date = DateTime.Now.AddDays(-new Random().Next(0, 5)) // Simulation de dates variées
        })
        .ToList();
    activites.AddRange(livres);

    // Derniers emprunts
    var emprunts = App.DbContext.Emprunts
        .OrderByDescending(e => e.DateEmprunt)
        .Take(3)
        .Select(e => new ActiviteRecente {
            Icone = "📖",
            Description = $"Emprunt : {e.Livre.Titre} par {e.Adherent.NomComplet}",
            Date = e.DateEmprunt
        })
        .ToList();
    activites.AddRange(emprunts);

    // Derniers retours
    var retours = App.DbContext.Emprunts
        .Where(e => e.DateRetour != null)
        .OrderByDescending(e => e.DateRetour)
        .Take(2)
        .Select(e => new ActiviteRecente {
            Icone = "🔄",
            Description = $"Retour : {e.Livre.Titre} par {e.Adherent.NomComplet}",
            Date = e.DateRetour.Value
        })
        .ToList();
    activites.AddRange(retours);

    // Nouveaux adhérents
    var adherents = App.DbContext.Adherents
        .OrderByDescending(a => a.Id)
        .Take(2)
        .Select(a => new ActiviteRecente {
            Icone = "👤",
            Description = $"Nouveau membre : {a.NomComplet}",
            Date = a.DateInscription
        })
        .ToList();
    activites.AddRange(adherents);

    // Trier par date décroissante
    activites = activites.OrderByDescending(a => a.Date).Take(10).ToList();

    // Créer une source de données avec regroupement
    var groupedItems = new ListCollectionView(activites);
    groupedItems.GroupDescriptions.Add(new PropertyGroupDescription("DateGroup"));
    
    // Affecter la source regroupée au contrôle
    ListeDernieresActivites.ItemsSource = groupedItems;
}
    }
}