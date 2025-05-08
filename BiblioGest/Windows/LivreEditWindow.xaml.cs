using System.Windows;
using BiblioGest.Models;

namespace BiblioGest.Windows;

public partial class LivreEditWindow : Window
{
    public Livre Livre { get; private set; }
    public string WindowTitle { get; private set; }
    public bool IsNewBook { get; private set; }

    public LivreEditWindow(Livre livre = null)
    {
        InitializeComponent();
        
        IsNewBook = livre == null;
        Livre = livre ?? new Livre();
        WindowTitle = IsNewBook ? "Ajouter un livre" : "Modifier un livre";
        
        // Charger les catégories
        CategorieComboBox.ItemsSource = App.DbContext.Categories.OrderBy(c => c.Nom).ToList();
        
        DataContext = this;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        // Validation des champs obligatoires
        if (string.IsNullOrWhiteSpace(Livre.ISBN) ||
            string.IsNullOrWhiteSpace(Livre.Titre) ||
            string.IsNullOrWhiteSpace(Livre.Auteur))
        {
            MessageBox.Show("Veuillez remplir tous les champs obligatoires.", 
                          "Erreur de validation", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Warning);
            return;
        }

        // Validation de l'année
        if (Livre.AnneePublication < 1000 || Livre.AnneePublication > DateTime.Now.Year)
        {
            MessageBox.Show("L'année de publication doit être valide.", 
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