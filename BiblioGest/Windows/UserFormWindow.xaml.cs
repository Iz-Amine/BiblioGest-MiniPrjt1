using System;
using System.Windows;
using System.Windows.Controls;
using BiblioGest.Models;

namespace BiblioGest.Windows
{
    public partial class UserFormWindow : Window
    {
        private readonly User _user;
        private readonly bool _isEditMode;

        public User User => _user;

        public UserFormWindow(User user = null)
        {
            InitializeComponent();
            _user = user ?? new User();
            _isEditMode = user != null;
            DataContext = this;

            if (_isEditMode)
            {
                TitleText.Text = "Modifier l'utilisateur";
                LoadUserData();
            }

            // Set default role if it's a new user
            if (user == null)
            {
                _user.Role = "Bibliothécaire";
            }
        }

        private void LoadUserData()
        {
            UsernameTextBox.Text = _user.Username;
            NomTextBox.Text = _user.Nom;
            PrenomTextBox.Text = _user.Prenom;
            EmailTextBox.Text = _user.Email;
            EstActifCheckBox.IsChecked = _user.EstActif;

            // Sélectionner le rôle dans la ComboBox
            foreach (ComboBoxItem item in RoleComboBox.Items)
            {
                if (item.Content.ToString() == _user.Role)
                {
                    RoleComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Recopie les valeurs des champs dans _user
            _user.Username = UsernameTextBox.Text;
            _user.Nom = NomTextBox.Text;
            _user.Prenom = PrenomTextBox.Text;
            _user.Email = EmailTextBox.Text;
            _user.Role = ((ComboBoxItem)RoleComboBox.SelectedItem)?.Content.ToString();
            _user.EstActif = EstActifCheckBox.IsChecked ?? true;

            // 2. Puis fais la validation
            // Validate required fields
            if (string.IsNullOrWhiteSpace(_user.Username) ||
                string.IsNullOrWhiteSpace(_user.Nom) ||
                string.IsNullOrWhiteSpace(_user.Prenom) ||
                string.IsNullOrWhiteSpace(_user.Email) ||
                string.IsNullOrWhiteSpace(_user.Role))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.", 
                    "Erreur de validation", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
                return;
            }

            // Validate email format
            if (!_user.Email.Contains("@"))
            {
                MessageBox.Show("Veuillez entrer une adresse email valide.", 
                    "Erreur de validation", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
                return;
            }

            // Validate role
            if (_user.Role != "Administrateur" && _user.Role != "Bibliothécaire")
            {
                MessageBox.Show("Le rôle doit être 'Administrateur' ou 'Bibliothécaire'.", 
                    "Erreur de validation", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                _user.Username = UsernameTextBox.Text;
                _user.Nom = NomTextBox.Text;
                _user.Prenom = PrenomTextBox.Text;
                _user.Email = EmailTextBox.Text;
                _user.Role = ((ComboBoxItem)RoleComboBox.SelectedItem).Content.ToString();
                _user.EstActif = EstActifCheckBox.IsChecked ?? true;

                if (!_isEditMode || !string.IsNullOrEmpty(PasswordBox.Password))
                {
                    // TODO: Implement proper password hashing
                    _user.PasswordHash = PasswordBox.Password;
                }

                if (_isEditMode)
                {
                    _user.DateModification = DateTime.Now;
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement : {ex.Message}", 
                    "Erreur", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 