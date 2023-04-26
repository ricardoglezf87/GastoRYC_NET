using BBDDLib.Services;
using GastosRYC.BBDDLib.Services;
using Microsoft.EntityFrameworkCore.Internal;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para SpashForm.xaml
    /// </summary>
    public partial class SpashForm : Window
    {   
        public SpashForm()
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
