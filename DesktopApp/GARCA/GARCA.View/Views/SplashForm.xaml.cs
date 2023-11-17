using System;
using System.Reflection;
using System.Windows;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.View.Views
{
    /// <summary>
    /// Lógica de interacción para SpashForm.xaml
    /// </summary>
    public partial class SplashForm : Window
    {
        public SplashForm()
        {
            InitializeComponent();
            lblVersion.Content = "Version: " + Assembly.GetExecutingAssembly().GetName().Version;
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            //TODO: crear funcion y realizar backup
            await iMigrationService.Migrate();
            new MainWindow().Show();
            Close();
        }

    }
}
