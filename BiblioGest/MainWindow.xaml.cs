﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BiblioGest.Pages;
using BiblioGest.Models;

namespace BiblioGest;


/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private User _currentUser;

    public MainWindow(User user)
    {
        InitializeComponent();
        _currentUser = user;
        UpdateNavbarForRole();
        // Naviguer vers la page du dashboard/statistiques par défaut
        NavigateToStatistiques(null, null);
    }

    private void UpdateNavbarForRole()
    {
        // Masquer le bouton de gestion des utilisateurs si non admin
        if (_currentUser.Role != "Administrateur")
        {
            BtnUsersNav.Visibility = Visibility.Collapsed;
        }
        else
        {
            BtnUsersNav.Visibility = Visibility.Visible;
        }
    }

    private void NavigateToLivres(object sender, RoutedEventArgs e)
    {
        try
        {
            MainFrame.Navigate(new Pages.LivresPage());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors de la navigation : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void NavigateToAdherents(object sender, RoutedEventArgs e)
    {
        try
        {
            MainFrame.Navigate(new Pages.AdherentsPage());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors de la navigation : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void NavigateToEmprunts(object sender, RoutedEventArgs e)
    {
        try
        {
            MainFrame.Navigate(new EmpruntsPage());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors de la navigation : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void NavigateToStatistiques(object sender, RoutedEventArgs e)
    {
        try
        {
            MainFrame.Navigate(new Pages.StatistiquesPage());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors de la navigation : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void NavigateToCategories(object sender, RoutedEventArgs e)
    {
        try
        {
            MainFrame.Navigate(new CategoriesPage());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors de la navigation : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        var loginWindow = new Windows.LoginWindow();
        loginWindow.Show();
        this.Close();
    }

    private void NavigateToUsers(object sender, RoutedEventArgs e)
    {
        try
        {
            MainFrame.Navigate(new Pages.UsersPage());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors de la navigation : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}