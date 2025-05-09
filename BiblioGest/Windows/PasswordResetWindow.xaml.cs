using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using BiblioGest.Models;

namespace BiblioGest.Windows
{
    public partial class PasswordResetWindow : Window
    {
        private readonly User _user;
        private bool _isPasswordValid;
        private bool _doPasswordsMatch;

        public PasswordResetWindow(User user)
        {
            InitializeComponent();
            _user = user;
            _isPasswordValid = false;
            _doPasswordsMatch = false;
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var password = NewPasswordBox.Password;
            UpdatePasswordRequirements(password);
            UpdatePasswordStrength(password);
            ValidatePasswords();
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidatePasswords();
        }

        private void UpdatePasswordRequirements(string password)
        {
            // Vérifier la longueur
            bool hasLength = password.Length >= 8;
            LengthRequirement.Foreground = hasLength ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Gray;

            // Vérifier les majuscules
            bool hasUpper = password.Any(char.IsUpper);
            UppercaseRequirement.Foreground = hasUpper ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Gray;

            // Vérifier les minuscules
            bool hasLower = password.Any(char.IsLower);
            LowercaseRequirement.Foreground = hasLower ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Gray;

            // Vérifier les chiffres
            bool hasNumber = password.Any(char.IsDigit);
            NumberRequirement.Foreground = hasNumber ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Gray;

            // Vérifier les caractères spéciaux
            bool hasSpecial = Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]");
            SpecialRequirement.Foreground = hasSpecial ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Gray;

            _isPasswordValid = hasLength && hasUpper && hasLower && hasNumber && hasSpecial;
        }

        private void UpdatePasswordStrength(string password)
        {
            int strength = 0;

            // Longueur
            if (password.Length >= 8) strength += 20;
            if (password.Length >= 12) strength += 10;

            // Complexité
            if (password.Any(char.IsUpper)) strength += 20;
            if (password.Any(char.IsLower)) strength += 20;
            if (password.Any(char.IsDigit)) strength += 20;
            if (Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]")) strength += 20;

            // Mettre à jour l'interface
            PasswordStrengthBar.Value = strength;
            
            // Mettre à jour le texte
            if (strength < 40)
            {
                PasswordStrengthText.Text = "Faible";
                PasswordStrengthBar.Foreground = System.Windows.Media.Brushes.Red;
            }
            else if (strength < 80)
            {
                PasswordStrengthText.Text = "Moyen";
                PasswordStrengthBar.Foreground = System.Windows.Media.Brushes.Orange;
            }
            else
            {
                PasswordStrengthText.Text = "Fort";
                PasswordStrengthBar.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private void ValidatePasswords()
        {
            string password = NewPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                _doPasswordsMatch = false;
                PasswordMatchText.Visibility = Visibility.Collapsed;
            }
            else if (password != confirmPassword)
            {
                _doPasswordsMatch = false;
                PasswordMatchText.Text = "Les mots de passe ne correspondent pas";
                PasswordMatchText.Visibility = Visibility.Visible;
            }
            else
            {
                _doPasswordsMatch = true;
                PasswordMatchText.Visibility = Visibility.Collapsed;
            }

            SaveButton.IsEnabled = _isPasswordValid && _doPasswordsMatch;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isPasswordValid || !_doPasswordsMatch)
                return;

            try
            {
                // TODO: Implement proper password hashing
                _user.PasswordHash = NewPasswordBox.Password;
                _user.DateModification = DateTime.Now;

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la réinitialisation du mot de passe : {ex.Message}", 
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