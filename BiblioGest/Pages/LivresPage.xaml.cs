using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BiblioGest.Models;
using BiblioGest.Windows;
using Microsoft.EntityFrameworkCore;

namespace BiblioGest.Pages;

public partial class LivresPage : Page
{
    private string _searchText = string.Empty;
    private int? _selectedCategoryId = null;
    
    public LivresPage()
    {
        InitializeComponent();
        LoadCategories();
        LoadLivres();
    }

    private void LoadCategories()
    {
        try
        {
            // Récupérer toutes les catégories
            var categories = App.DbContext.Categories
                .OrderBy(c => c.Nom)
                .ToList();

            // Ajouter l'option "Toutes les catégories" au début
            var allCategories = new List<Categorie>
            {
                new Categorie { Id = -1, Nom = "Toutes les catégories" }
            };
            allCategories.AddRange(categories);

            // Définir la source de données
            CategorieComboBox.ItemsSource = allCategories;
            CategorieComboBox.SelectedIndex = 0; // Sélectionner "Toutes les catégories" par défaut
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors du chargement des catégories : {ex.Message}", 
                          "Erreur", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Error);
        }
    }

    private void LoadLivres()
    {
        try
        {
            // Construire la requête en fonction des filtres
            var query = App.DbContext.Livres
                .Include(l => l.Categorie)
                .AsQueryable();

            // Appliquer le filtre de recherche par texte
            if (!string.IsNullOrEmpty(_searchText))
            {
                query = query.Where(l => 
                    l.Titre.ToLower().Contains(_searchText) ||
                    l.Auteur.ToLower().Contains(_searchText) ||
                    l.ISBN.Contains(_searchText));
            }

            // Appliquer le filtre par catégorie
            if (_selectedCategoryId.HasValue && _selectedCategoryId.Value > 0)
            {
                query = query.Where(l => l.CategorieId == _selectedCategoryId.Value);
            }

            // Trier et exécuter la requête
            var livres = query.OrderBy(l => l.Titre).ToList();
            
            // Mettre à jour la source de données de la grille
            LivresGrid.ItemsSource = livres;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors du chargement des livres : {ex.Message}", 
                          "Erreur", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Error);
        }
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _searchText = SearchBox.Text.ToLower();
        LoadLivres();
    }

    private void CategorieComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CategorieComboBox.SelectedItem is Categorie selectedCategorie)
        {
            _selectedCategoryId = selectedCategorie.Id <= 0 ? null : selectedCategorie.Id;
            LoadLivres();
        }
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