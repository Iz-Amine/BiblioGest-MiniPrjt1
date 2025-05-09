using System;
using System.Linq;
using System.Windows;
using BiblioGest.Models;
using Microsoft.EntityFrameworkCore;

namespace BiblioGest.Windows;

public partial class EmpruntEditWindow : Window
{
    public Emprunt Emprunt { get; private set; }
    public string WindowTitle { get; private set; }
    public bool IsNewEmprunt { get; private set; }

    public EmpruntEditWindow(Emprunt emprunt = null)
    {
        InitializeComponent();
        
        IsNewEmprunt = emprunt == null;
        Emprunt = emprunt ?? new Emprunt { 
            DateEmprunt = DateTime.Today,
            DateRetourPrevue = DateTime.Today.AddDays(14) // Ajouter une date de retour par défaut (14 jours)
        };
        WindowTitle = IsNewEmprunt ? "Nouvel emprunt" : "Modifier un emprunt";
        
        // Charger les livres disponibles
        LivreComboBox.ItemsSource = App.DbContext.Livres
            .Where(l => l.Disponible == true || (!IsNewEmprunt && l.Id == Emprunt.LivreId))
            .ToList();

        // Charger les adhérents
        AdherentComboBox.ItemsSource = App.DbContext.Adherents.ToList();
        
        DataContext = this;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        // Validation des champs obligatoires
        if (Emprunt.Livre == null || Emprunt.Adherent == null || Emprunt.DateEmprunt == default)
        {
            MessageBox.Show("Veuillez remplir tous les champs obligatoires.", 
                          "Erreur de validation", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Warning);
            return;
        }

        // Validation des dates
        if (Emprunt.DateRetour.HasValue && Emprunt.DateRetour.Value < Emprunt.DateEmprunt)
        {
            MessageBox.Show("La date de retour ne peut pas être antérieure à la date d'emprunt.", 
                          "Erreur de validation", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Warning);
            return;
        }

        // Mise à jour du statut du livre
        if (IsNewEmprunt)
        {
            Emprunt.Livre.Disponible = false;
        }
        else if (Emprunt.DateRetour.HasValue)
        {
            Emprunt.Livre.Disponible = true;
        }

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void SearchByIsbn_Click(object sender, RoutedEventArgs e)
    {
        var isbn = IsbnTextBox.Text.Trim();
        if (string.IsNullOrEmpty(isbn))
        {
            MessageBox.Show("Veuillez saisir un ISBN.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var livre = App.DbContext.Livres.FirstOrDefault(l => l.ISBN == isbn);
        if (livre != null)
        {
            LivreComboBox.SelectedItem = livre;
        }
        else
        {
            MessageBox.Show($"Aucun livre trouvé avec l'ISBN {isbn}.", "Non trouvé", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}