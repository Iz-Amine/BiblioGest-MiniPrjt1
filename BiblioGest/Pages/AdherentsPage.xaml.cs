using System.Windows;
using System.Windows.Controls;
using System.Linq;
using BiblioGest.Models;
using BiblioGest.Windows;
using Microsoft.EntityFrameworkCore;

namespace BiblioGest.Pages;

public partial class AdherentsPage : Page
{
    private int _currentPage = 1;
    private int _pageSize = 10;
    private int _totalPages = 1;

    public AdherentsPage()
    {
        InitializeComponent();
        LoadAdherents();
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

    private void LoadAdherents()
    {
        try
        {
            var query = App.DbContext.Adherents.AsQueryable();
            if (!string.IsNullOrEmpty(SearchBox.Text))
            {
                var searchText = SearchBox.Text.ToLower();
                query = query.Where(a => a.Nom.ToLower().Contains(searchText) || a.Prenom.ToLower().Contains(searchText) || a.Email.ToLower().Contains(searchText));
            }
            int totalItems = query.Count();
            UpdatePagination(totalItems);
            var adherents = query.OrderBy(a => a.Nom).ThenBy(a => a.Prenom)
                                  .Skip((_currentPage - 1) * _pageSize)
                                  .Take(_pageSize)
                                  .ToList();
            AdherentsGrid.ItemsSource = adherents;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors du chargement des adhérents : {ex.Message}",
                          "Erreur",
                          MessageBoxButton.OK,
                          MessageBoxImage.Error);
        }
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

    private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            LoadAdherents();
        }
    }

    private void BtnNextPage_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage < _totalPages)
        {
            _currentPage++;
            LoadAdherents();
        }
    }
} 