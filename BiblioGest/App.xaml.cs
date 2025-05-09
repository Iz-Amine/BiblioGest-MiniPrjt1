using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using BiblioGest.Data;

namespace BiblioGest;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static BibliothequeContext DbContext { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            // Créer un dossier Data dans le répertoire de l'application
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dataFolder = Path.Combine(appDirectory, "Data");
            
            // S'assurer que le dossier existe
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
                MessageBox.Show($"Dossier de données créé : {dataFolder}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
            string dbPath = Path.Combine(dataFolder, "bibliotheque12.db");
            Console.WriteLine(dbPath);
            string connectionString = $"Data Source={dbPath}";

            var options = new DbContextOptionsBuilder<BibliothequeContext>()
                .UseSqlite(connectionString)
                .Options;

            DbContext = new BibliothequeContext(options);
            // // Appeler le seeder ici
            // Seeder.Seed(DbContext);

            DbContext.Database.EnsureCreated();

            MessageBox.Show($"Base de données initialisée avec succès : {dbPath}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors de l'initialisation de la base de données : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        DbContext?.Dispose();
    }
}