using System.Windows;
using System.Text.RegularExpressions;
using BiblioGest.Models;
using System.Windows.Controls;

namespace BiblioGest.Windows;

public partial class AdherentEditWindow : Window
{
    public Adherent Adherent { get; private set; }
    public string WindowTitle { get; private set; }
    public bool IsNewAdherent { get; private set; }

    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    private static readonly Regex PhoneRegex = new(@"^[0-9\s\-\+\(\)]{10,}$");

    public AdherentEditWindow(Adherent adherent = null)
    {
        InitializeComponent();
        
        IsNewAdherent = adherent == null;
        Adherent = adherent ?? new Adherent();
        WindowTitle = IsNewAdherent ? "Ajouter un adhérent" : "Modifier un adhérent";
        
        DataContext = this;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        // Validation des champs obligatoires
        if (string.IsNullOrWhiteSpace(Adherent.Nom) ||
            string.IsNullOrWhiteSpace(Adherent.Prenom) ||
            string.IsNullOrWhiteSpace(Adherent.Email))
        {
            MessageBox.Show("Veuillez remplir tous les champs obligatoires.", 
                          "Erreur de validation", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Warning);
            return;
        }

        // Forcer la récupération du statut depuis le ComboBox
        if (StatutComboBox.SelectedItem is ComboBoxItem item)
        {
            Adherent.Statut = item.Content.ToString();
        }
        else if (string.IsNullOrWhiteSpace(Adherent.Statut))
        {
            Adherent.Statut = "Actif";
        }

        // Validation de l'email
        if (!EmailRegex.IsMatch(Adherent.Email))
        {
            MessageBox.Show("L'adresse email n'est pas valide.", 
                          "Erreur de validation", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Warning);
            return;
        }

        // Validation du téléphone (si renseigné)
        if (!string.IsNullOrWhiteSpace(Adherent.Telephone) && !PhoneRegex.IsMatch(Adherent.Telephone))
        {
            MessageBox.Show("Le numéro de téléphone n'est pas valide.", 
                          "Erreur de validation", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Warning);
            return;
        }

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
} 