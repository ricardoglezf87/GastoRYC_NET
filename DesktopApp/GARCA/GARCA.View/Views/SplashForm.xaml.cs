using System;
using System.Reflection;
using System.Threading.Tasks;
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

        private async Task ComprobateDataBase()
        {
            iRycContextService.MakeBackup();
            await iMigrationService.Migrate();
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            await ComprobateDataBase();
            new MainWindow().Show();
            Close();
        }

    }
}
