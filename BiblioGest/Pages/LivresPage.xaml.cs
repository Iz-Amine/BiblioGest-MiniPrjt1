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
    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalPages = 1;
    
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

    private void UpdatePagination(int totalItems)
    {
        _totalPages = (int)Math.Ceiling((double)totalItems / _pageSize);
        if (_currentPage > _totalPages) _currentPage = _totalPages;
        if (_currentPage < 1) _currentPage = 1;
        TxtPageInfo.Text = $"Page {_currentPage}/{(_totalPages == 0 ? 1 : _totalPages)}";
        BtnPrevPage.IsEnabled = _currentPage > 1;
        BtnNextPage.IsEnabled = _currentPage < _totalPages;
    }

    private void LoadLivres()
    {
        try
        {
            var query = App.DbContext.Livres
                .Include(l => l.Categorie)
                .AsQueryable();

            if (!string.IsNullOrEmpty(_searchText))
            {
                query = query.Where(l =>
                    l.Titre.ToLower().Contains(_searchText) ||
                    l.Auteur.ToLower().Contains(_searchText) ||
                    l.ISBN.Contains(_searchText));
            }

            if (_selectedCategoryId.HasValue && _selectedCategoryId.Value > 0)
            {
                query = query.Where(l => l.CategorieId == _selectedCategoryId.Value);
            }

            int totalItems = query.Count();
            UpdatePagination(totalItems);

            var livres = query
                .OrderBy(l => l.Titre)
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();

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

    private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            LoadLivres();
        }
    }

    private void BtnNextPage_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage < _totalPages)
        {
            _currentPage++;
            LoadLivres();
        }
    }
}