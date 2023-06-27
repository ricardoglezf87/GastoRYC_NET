using GARCA.BO.Extensions;
using GARCA.BO.Models;
using GARCA.BO.ModelsView;
using GARCA.BO.Services;
using GARCA.ViewModels;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static GARCA.BO.Extensions.WindowsExtension;

namespace GARCA.Views
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
            gvPortfolio.ExpandAllGroup();
        }

        #endregion       
    }
}
