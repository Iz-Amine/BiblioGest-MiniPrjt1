using System.Windows;
using System.Windows.Controls;
using System.Linq;
using BiblioGest.Models;
using BiblioGest.Windows;
using Microsoft.EntityFrameworkCore;

namespace BiblioGest.Pages;

public partial class CategoriesPage : Page
{
    public CategoriesPage()
    {
        InitializeComponent();
        LoadCategories();
    }

    private void LoadCategories()
    {
        try
        {
            var categories = App.DbContext.Categories
                .OrderBy(c => c.Nom)
                .ToList();
            CategoriesGrid.ItemsSource = categories;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors du chargement des catégories : {ex.Message}", 
                          "Erreur", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Error);
        }
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            var searchText = SearchBox.Text?.ToLower() ?? string.Empty;
            var categories = App.DbContext.Categories
                .Where(c => c.Nom.ToLower().Contains(searchText) ||
                           (c.Description != null && c.Description.ToLower().Contains(searchText)))
                .OrderBy(c => c.Nom)
                .ToList();
            CategoriesGrid.ItemsSource = categories;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur de recherche : {ex.Message}");
        }
    }

    private void AddCategorie_Click(object sender, RoutedEventArgs e)
    {
        var window = new CategorieEditWindow
        {
            Owner = Window.GetWindow(this)
        };

        if (window.ShowDialog() == true)
        {
            try
            {
                App.DbContext.Categories.Add(window.Categorie);
                App.DbContext.SaveChanges();
                LoadCategories();
                MessageBox.Show("Catégorie ajoutée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout de la catégorie : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void CategoriesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CategoriesGrid.SelectedItem is Categorie selectedCategorie)
        {
            var window = new CategorieEditWindow(selectedCategorie)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() == true)
            {
                try
                {
                    App.DbContext.SaveChanges();
                    LoadCategories();
                    MessageBox.Show("Catégorie modifiée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la modification de la catégorie : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void DeleteCategorie_Click(object sender, RoutedEventArgs e)
    {
        if (CategoriesGrid.SelectedItem is Categorie selectedCategorie)
        {
            // Vérifier si la catégorie est utilisée par des livres
            var livresCount = App.DbContext.Livres.Count(l => l.CategorieId == selectedCategorie.Id);
            if (livresCount > 0)
            {
                MessageBox.Show(
                    $"Impossible de supprimer cette catégorie car elle est utilisée par {livresCount} livre(s).",
                    "Suppression impossible",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer la catégorie '{selectedCategorie.Nom}' ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    App.DbContext.Categories.Remove(selectedCategorie);
                    App.DbContext.SaveChanges();
                    LoadCategories();
                    MessageBox.Show("Catégorie supprimée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression de la catégorie : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
} 