using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.View.Views
{
    /// <summary>
    /// Lógica de interacción para partialPortfolio.xaml
    /// </summary>
    public partial class PartialPortfolio : Page
    {

        #region Constructor

        public PartialPortfolio()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPortfolio();
        }

        #endregion

        #region Functions        

        public async Task LoadPortfolio()
        {
            try
            {
                gvPortfolio.ItemsSource = await iVPortfolioService.GetAllAsync();
                gvPortfolio.ExpandAllGroup();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GARCA");
            }
        }

        #endregion       
    }
}
