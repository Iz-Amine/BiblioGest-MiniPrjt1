using System.Windows;
using BiblioGest.Models;

namespace BiblioGest.Windows;

public partial class CategorieEditWindow : Window
{
    public Categorie Categorie { get; private set; }
    public string WindowTitle { get; private set; }
    public bool IsNewCategorie { get; private set; }

    public CategorieEditWindow(Categorie categorie = null)
    {
        InitializeComponent();
        
        IsNewCategorie = categorie == null;
        Categorie = categorie ?? new Categorie();
        WindowTitle = IsNewCategorie ? "Nouvelle catégorie" : "Modifier une catégorie";
        
        DataContext = this;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        // Validation des champs obligatoires
        if (string.IsNullOrWhiteSpace(Categorie.Nom))
        {
            MessageBox.Show("Veuillez saisir un nom pour la catégorie.", 
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