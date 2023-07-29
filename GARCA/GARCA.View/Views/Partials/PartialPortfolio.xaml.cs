using GARCA.BO.Services;
using System.Windows;
using System.Windows.Controls;
using GARCA.Utils.IOC;

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
            gvPortfolio.ItemsSource = await DependencyConfig.iVPortfolioService.getAllAsync();
            gvPortfolio.ExpandAllGroup();
        }

        #endregion       
    }
}
