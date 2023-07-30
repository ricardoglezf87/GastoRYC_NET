using GARCA.BO.Services;
using System.Windows;
using System.Windows.Controls;
using GARCA.Utils.IOC;

namespace GARCA.View.Views
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

        public PartialPortfolio(MainWindow parentForm)
        {
            InitializeComponent();
            this.parentForm = parentForm;
        }

        #endregion

        #region Events

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPortfolio();
        }

        #endregion

        #region Functions        

        public async void LoadPortfolio()
        {
            gvPortfolio.ItemsSource = await DependencyConfig.IVPortfolioService.GetAllAsync();
            gvPortfolio.ExpandAllGroup();
        }

        #endregion       
    }
}
