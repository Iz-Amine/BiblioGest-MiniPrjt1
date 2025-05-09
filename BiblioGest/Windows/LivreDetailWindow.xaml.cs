using System.Linq;
using System.Windows;
using BiblioGest.Models;
using Microsoft.EntityFrameworkCore;

namespace BiblioGest.Windows
{
    public partial class LivreDetailWindow : Window
    {
        public Livre Livre { get; set; }
        public LivreDetailWindow(Livre livre)
        {
            InitializeComponent();
            Livre = App.DbContext.Livres
                .Include(l => l.Categorie)
                .FirstOrDefault(l => l.Id == livre.Id) ?? livre;
            DataContext = this;
            LoadHistorique();
        }

        private void LoadHistorique()
        {
            var historique = App.DbContext.Emprunts
                .Include(e => e.Adherent)
                .Where(e => e.LivreId == Livre.Id)
                .OrderByDescending(e => e.DateEmprunt)
                .ToList();
            EmpruntsGrid.ItemsSource = historique;
        }

        private void FermerButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 