using System;
using System.Reflection;
using System.Windows;

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
            lblVersion.Content = "Version: " + Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            new MainWindow().Show();
            Close();
        }

    }
}
