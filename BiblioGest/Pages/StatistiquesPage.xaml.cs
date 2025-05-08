using System;
using System.Windows.Controls;
using System.Windows;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BiblioGest.Pages
{
    public partial class StatistiquesPage : Page
    {
        public StatistiquesPage()
        {
            InitializeComponent();
            ChargerStatistiques();
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
                var livresDisponibles = App.DbContext.Livres.Count(l => l.Disponible);
                var livresEmpruntes = totalLivres - livresDisponibles;

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
    }
}