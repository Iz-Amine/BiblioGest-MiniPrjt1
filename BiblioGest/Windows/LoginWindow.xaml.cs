using System.Windows;
using System.Linq;
using BiblioGest.Models;

namespace BiblioGest.Windows
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            // Recherche de l'utilisateur dans la base
            var user = App.DbContext.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                MessageBox.Show("Utilisateur inexistant.", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Vérification du mot de passe (ici, sans hash pour l'instant)
            if (user.PasswordHash != password)
            {
                MessageBox.Show("Mot de passe incorrect.", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Connexion réussie
            user.DerniereConnexion = System.DateTime.Now;
            App.DbContext.SaveChanges();

            MainWindow mainWindow = new MainWindow(user);
            mainWindow.Show();
            this.Close();
        }
    }
} 