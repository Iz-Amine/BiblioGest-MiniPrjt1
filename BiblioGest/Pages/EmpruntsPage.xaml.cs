using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BiblioGest.Models;
using BiblioGest.Windows;
using Microsoft.EntityFrameworkCore;

namespace BiblioGest.Pages;

public partial class EmpruntsPage : Page
{
    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalPages = 1;
    private string _selectedStatut = "Tous";

    public EmpruntsPage()
    {
        InitializeComponent();
        LoadEmprunts();
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

    private void LoadEmprunts()
    {
        try
        {
            var query = App.DbContext.Emprunts
                .Include(e => e.Livre)
                .Include(e => e.Adherent)
                .AsQueryable();
            if (!string.IsNullOrEmpty(SearchBox.Text))
            {
                var searchText = SearchBox.Text.ToLower();
                query = query.Where(e => (e.Livre != null && e.Livre.Titre != null && e.Livre.Titre.ToLower().Contains(searchText)) ||
                                        (e.Adherent != null && e.Adherent.Nom != null && e.Adherent.Nom.ToLower().Contains(searchText)) ||
                                        (e.Adherent != null && e.Adherent.Prenom != null && e.Adherent.Prenom.ToLower().Contains(searchText)));
            }
            // Charger en mémoire pour filtrer sur EstEnRetard
            var empruntsList = query.OrderByDescending(e => e.DateEmprunt).ToList();
            if (_selectedStatut != "Tous")
            {
                if (_selectedStatut == "En cours")
                    empruntsList = empruntsList.Where(e => e.DateRetour == null && !e.EstEnRetard).ToList();
                else if (_selectedStatut == "En retard")
                    empruntsList = empruntsList.Where(e => e.DateRetour == null && e.EstEnRetard).ToList();
                else if (_selectedStatut == "Rendu")
                    empruntsList = empruntsList.Where(e => e.DateRetour != null).ToList();
            }
            int totalItems = empruntsList.Count;
            UpdatePagination(totalItems);
            var empruntsPage = empruntsList
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();
            EmpruntsGrid.ItemsSource = empruntsPage;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors du chargement des emprunts : {ex.Message}", 
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
            
            var emprunts = App.DbContext.Emprunts
                .Include(e => e.Livre)
                .Include(e => e.Adherent)
                .Where(e => (e.Livre != null && e.Livre.Titre != null && e.Livre.Titre.ToLower().Contains(searchText)) ||
                           (e.Adherent != null && e.Adherent.Nom != null && e.Adherent.Nom.ToLower().Contains(searchText)) ||
                           (e.Adherent != null && e.Adherent.Prenom != null && e.Adherent.Prenom.ToLower().Contains(searchText)))
                .OrderByDescending(e => e.DateEmprunt)
                .ToList();
                
            EmpruntsGrid.ItemsSource = emprunts;
        }
        catch (Exception ex)
        {
            // Option 1: Journaliser l'erreur silencieusement
            Console.WriteLine($"Erreur de recherche : {ex.Message}");
            
            // Option 2: Informer l'utilisateur et revenir à la liste complète
            // MessageBox.Show($"Erreur lors de la recherche : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            // LoadEmprunts();
        }
    }

    private void AddEmprunt_Click(object sender, RoutedEventArgs e)
    {
        var window = new EmpruntEditWindow
        {
            Owner = Window.GetWindow(this)
        };

        if (window.ShowDialog() == true)
        {
            try
            {
                // S'assurer que les entités sont chargées et suivies correctement
                if (window.Emprunt.Livre != null)
                {
                    var livre = App.DbContext.Livres.Find(window.Emprunt.Livre.Id);
                    if (livre != null)
                    {
                        livre.Disponible = false;
                        window.Emprunt.Livre = livre;
                        window.Emprunt.LivreId = livre.Id;
                    }
                }

                App.DbContext.Emprunts.Add(window.Emprunt);
                App.DbContext.SaveChanges();
                LoadEmprunts();
                MessageBox.Show("Emprunt ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout de l'emprunt : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void EmpruntsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (EmpruntsGrid.SelectedItem is Emprunt selectedEmprunt)
        {
            var window = new EmpruntEditWindow(selectedEmprunt)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() == true)
            {
                try
                {
                    // S'assurer que les entités sont chargées et suivies correctement
                    var livre = App.DbContext.Livres.Find(window.Emprunt.LivreId);
                    if (livre != null)
                    {
                        // Si l'emprunt est retourné, mettre à jour le statut du livre
                        if (window.Emprunt.DateRetour.HasValue)
                        {
                            livre.Disponible = true;
                        }
                    }

                    App.DbContext.SaveChanges();
                    LoadEmprunts();
                    MessageBox.Show("Emprunt modifié avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la modification de l'emprunt : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void DeleteEmprunt_Click(object sender, RoutedEventArgs e)
    {
        if (EmpruntsGrid.SelectedItem is Emprunt selectedEmprunt)
        {
            var result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer l'emprunt du livre '{selectedEmprunt.Livre?.Titre ?? "inconnu"}' ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Remettre le livre en disponible
                    var livre = App.DbContext.Livres.Find(selectedEmprunt.LivreId);
                    if (livre != null)
                    {
                        livre.Disponible = true;
                    }
                    
                    App.DbContext.Emprunts.Remove(selectedEmprunt);
                    App.DbContext.SaveChanges();
                    LoadEmprunts();
                    MessageBox.Show("Emprunt supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression de l'emprunt : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            LoadEmprunts();
        }
    }

    private void BtnNextPage_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage < _totalPages)
        {
            _currentPage++;
            LoadEmprunts();
        }
    }

    private void StatutComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (StatutComboBox.SelectedItem is ComboBoxItem item)
        {
            _selectedStatut = item.Content.ToString();
            LoadEmprunts();
        }
    }
}