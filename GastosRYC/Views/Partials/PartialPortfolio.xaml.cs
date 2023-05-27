using BOLib.Extensions;
using BOLib.Models;
using BOLib.ModelsView;
using BOLib.Services;
using GastosRYC.ViewModels;
using Syncfusion.Data.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static BOLib.Extensions.WindowsExtension;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para partialPortfolio.xaml
    /// </summary>
    public partial class PartialPortfolio : Page
    {

        #region Variables

        private readonly MainWindow parentForm;

        #endregion

        #region Constructor

        public PartialPortfolio(MainWindow _parentForm)
        {
            InitializeComponent();
            parentForm = _parentForm;
        }

        #endregion

        #region Events

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadPortfolio();
        }

        #endregion

        #region Functions        

        public async void loadPortfolio()
        {
            gvPortfolio.ItemsSource = await VPortfolioService.Instance.getAllAsync();            
        }
        #endregion       
    }
}
