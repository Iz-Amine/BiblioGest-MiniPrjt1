using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BiblioGest.Models;
using BiblioGest.Services;
using BiblioGest.Windows;

namespace BiblioGest.Pages
{
    public partial class UsersPage : Page
    {
        private readonly UserService _userService;
        private List<User> _users;
        private int _currentPage = 1;
        private const int ItemsPerPage = 10;

        public UsersPage()
        {
            InitializeComponent();
            _userService = new UserService();
            LoadUsers();
        }

        private async void LoadUsers()
        {
            try
            {
                _users = await _userService.GetAllUsersAsync();
                RefreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des utilisateurs : {ex.Message}", 
                    "Erreur", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        private void RefreshGrid()
        {
            var searchText = SearchBox.Text?.ToLower() ?? "";
            var filteredUsers = _users
                .Where(u => string.IsNullOrEmpty(searchText) ||
                           u.Username.ToLower().Contains(searchText) ||
                           u.Nom.ToLower().Contains(searchText) ||
                           u.Prenom.ToLower().Contains(searchText) ||
                           u.Email.ToLower().Contains(searchText))
                .Skip((_currentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToList();

            UsersGrid.ItemsSource = filteredUsers;
            UpdatePagination();
        }

        private void UpdatePagination()
        {
            var searchText = SearchBox.Text?.ToLower() ?? "";
            var filteredCount = _users.Count(u => string.IsNullOrEmpty(searchText) ||
                                                u.Username.ToLower().Contains(searchText) ||
                                                u.Nom.ToLower().Contains(searchText) ||
                                                u.Prenom.ToLower().Contains(searchText) ||
                                                u.Email.ToLower().Contains(searchText));
            
            int totalPages = (int)Math.Ceiling(filteredCount / (double)ItemsPerPage);
            TxtPageInfo.Text = $"Page {_currentPage}/{totalPages}";
            BtnPrevPage.IsEnabled = _currentPage > 1;
            BtnNextPage.IsEnabled = _currentPage < totalPages;
        }

        private async void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _users = await _userService.SearchUsersAsync(SearchBox.Text);
                _currentPage = 1;
                RefreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche : {ex.Message}", 
                    "Erreur", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        private async void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var userForm = new UserFormWindow();
            if (userForm.ShowDialog() == true)
            {
                try
                {
                    var success = await _userService.CreateUserAsync(userForm.User);
                    if (success)
                    {
                        _users = await _userService.GetAllUsersAsync();
                        RefreshGrid();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la création de l'utilisateur.", 
                            "Erreur", 
                            MessageBoxButton.OK, 
                            MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la création de l'utilisateur : {ex.Message}", 
                        "Erreur", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                }
            }
        }

        private async void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is User selectedUser)
            {
                var result = MessageBox.Show(
                    "Êtes-vous sûr de vouloir supprimer cet utilisateur ?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var success = await _userService.DeleteUserAsync(selectedUser.Id);
                        if (success)
                        {
                            _users = await _userService.GetAllUsersAsync();
                            RefreshGrid();
                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de la suppression de l'utilisateur.", 
                                "Erreur", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la suppression de l'utilisateur : {ex.Message}", 
                            "Erreur", 
                            MessageBoxButton.OK, 
                            MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un utilisateur à supprimer.");
            }
        }

        private async void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is User user)
            {
                var userForm = new UserFormWindow(user);
                if (userForm.ShowDialog() == true)
                {
                    try
                    {
                        var success = await _userService.UpdateUserAsync(userForm.User);
                        if (success)
                        {
                            _users = await _userService.GetAllUsersAsync();
                            RefreshGrid();
                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de la modification de l'utilisateur.", 
                                "Erreur", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la modification de l'utilisateur : {ex.Message}", 
                            "Erreur", 
                            MessageBoxButton.OK, 
                            MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is User user)
            {
                var passwordWindow = new PasswordResetWindow(user);
                if (passwordWindow.ShowDialog() == true)
                {
                    try
                    {
                        var success = await _userService.UpdatePasswordAsync(user.Id, user.PasswordHash);
                        if (!success)
                        {
                            MessageBox.Show("Erreur lors de la réinitialisation du mot de passe.", 
                                "Erreur", 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la réinitialisation du mot de passe : {ex.Message}", 
                            "Erreur", 
                            MessageBoxButton.OK, 
                            MessageBoxImage.Error);
                    }
                }
            }
        }

        private void UsersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: Implement selection changed functionality if needed
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                RefreshGrid();
            }
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            var searchText = SearchBox.Text?.ToLower() ?? "";
            var filteredCount = _users.Count(u => string.IsNullOrEmpty(searchText) ||
                                                u.Username.ToLower().Contains(searchText) ||
                                                u.Nom.ToLower().Contains(searchText) ||
                                                u.Prenom.ToLower().Contains(searchText) ||
                                                u.Email.ToLower().Contains(searchText));
            
            int totalPages = (int)Math.Ceiling(filteredCount / (double)ItemsPerPage);
            if (_currentPage < totalPages)
            {
                _currentPage++;
                RefreshGrid();
            }
        }
    }
} 