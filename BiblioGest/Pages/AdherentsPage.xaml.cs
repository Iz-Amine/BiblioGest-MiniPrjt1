using System.Windows;
using System.Windows.Controls;
using System.Linq;
using BiblioGest.Models;
using BiblioGest.Windows;
using Microsoft.EntityFrameworkCore;

namespace BiblioGest.Pages;

public partial class AdherentsPage : Page
{
    public AdherentsPage()
    {
        InitializeComponent();
        LoadAdherents();
    }

    private void LoadAdherents()
    {
        var adherents = App.DbContext.Adherents
            .OrderBy(a => a.Nom)
            .ThenBy(a => a.Prenom)
            .ToList();
        AdherentsGrid.ItemsSource = adherents;
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = SearchBox.Text.ToLower();
        var adherents = App.DbContext.Adherents
            .Where(a => a.Nom.ToLower().Contains(searchText) ||
                       a.Prenom.ToLower().Contains(searchText) ||
                       a.Email.ToLower().Contains(searchText))
            .OrderBy(a => a.Nom)
            .ThenBy(a => a.Prenom)
            .ToList();
        AdherentsGrid.ItemsSource = adherents;
    }

    private void AddAdherent_Click(object sender, RoutedEventArgs e)
    {
        var window = new AdherentEditWindow
        {
            Owner = Window.GetWindow(this)
        };

        if (window.ShowDialog() == true)
        {
            try
            {
                App.DbContext.Adherents.Add(window.Adherent);
                App.DbContext.SaveChanges();
                LoadAdherents();
                MessageBox.Show("Adhérent ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout de l'adhérent : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void AdherentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AdherentsGrid.SelectedItem is Adherent selectedAdherent)
        {
            var window = new AdherentEditWindow(selectedAdherent)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() == true)
            {
                try
                {
                    App.DbContext.SaveChanges();
                    LoadAdherents();
                    MessageBox.Show("Adhérent modifié avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la modification de l'adhérent : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void DeleteAdherent_Click(object sender, RoutedEventArgs e)
    {
        if (AdherentsGrid.SelectedItem is Adherent selectedAdherent)
        {
            var result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer l'adhérent '{selectedAdherent.Nom} {selectedAdherent.Prenom}' ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    App.DbContext.Adherents.Remove(selectedAdherent);
                    App.DbContext.SaveChanges();
                    LoadAdherents();
                    MessageBox.Show("Adhérent supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression de l'adhérent : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
} 