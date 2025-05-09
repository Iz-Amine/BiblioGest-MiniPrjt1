using System.Linq;
using System.Windows;
using BiblioGest.Models;
using Microsoft.EntityFrameworkCore;

namespace BiblioGest.Windows
{
    public partial class AdherentDetailWindow : Window
    {
        public Adherent Adherent { get; set; }
        public AdherentDetailWindow(Adherent adherent)
        {
            InitializeComponent();
            Adherent = App.DbContext.Adherents
                .FirstOrDefault(a => a.Id == adherent.Id) ?? adherent;
            DataContext = this;
            LoadHistorique();
        }

        private void LoadHistorique()
        {
            var historique = App.DbContext.Emprunts
                .Include(e => e.Livre)
                .Where(e => e.AdherentId == Adherent.Id)
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