using System.Windows;
using System.Windows.Controls;
using System.Linq;
using BiblioGest.Models;
using BiblioGest.Windows;
using Microsoft.EntityFrameworkCore;

namespace BiblioGest.Pages;

public partial class LivresPage : Page
{
    public LivresPage()
    {
        InitializeComponent();
        LoadLivres();
    }

    private void LoadLivres()
    {
        var livres = App.DbContext.Livres
            .OrderBy(l => l.Titre)
            .ToList();
        LivresGrid.ItemsSource = livres;
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = SearchBox.Text.ToLower();
        var livres = App.DbContext.Livres
            .Where(l => l.Titre.ToLower().Contains(searchText) ||
                       l.Auteur.ToLower().Contains(searchText) ||
                       l.ISBN.Contains(searchText))
            .OrderBy(l => l.Titre)
            .ToList();
        LivresGrid.ItemsSource = livres;
    }

    private void AddBook_Click(object sender, RoutedEventArgs e)
    {
        var window = new LivreEditWindow
        {
            Owner = Window.GetWindow(this)
        };

        if (window.ShowDialog() == true)
        {
            try
            {
                App.DbContext.Livres.Add(window.Livre);
                App.DbContext.SaveChanges();
                LoadLivres();
                MessageBox.Show("Livre ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout du livre : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void LivresGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LivresGrid.SelectedItem is Livre selectedLivre)
        {
            var window = new LivreEditWindow(selectedLivre)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() == true)
            {
                try
                {
                    App.DbContext.SaveChanges();
                    LoadLivres();
                    MessageBox.Show("Livre modifié avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la modification du livre : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void DeleteBook_Click(object sender, RoutedEventArgs e)
    {
        if (LivresGrid.SelectedItem is Livre selectedLivre)
        {
            var result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer le livre '{selectedLivre.Titre}' ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    App.DbContext.Livres.Remove(selectedLivre);
                    App.DbContext.SaveChanges();
                    LoadLivres();
                    MessageBox.Show("Livre supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression du livre : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
} 