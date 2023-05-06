using BBDDLib.Models;
using GastosRYC.BBDDLib.Services;
using System.Windows;
using System.Windows.Controls;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para InvestmentProducts.xaml
    /// </summary>
    public partial class FrmInvestmentProductsList : Window
    {
        private readonly SimpleInjector.Container servicesContainer;

        public FrmInvestmentProductsList(SimpleInjector.Container servicesContainer)
        {
            InitializeComponent();
            this.servicesContainer = servicesContainer;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gvInvestmentProducts.ItemsSource = servicesContainer.GetInstance<InvestmentProductsService>().getAll();
        }

        private void gvInvestmentProducts_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            InvestmentProducts InvestmentProducts = (InvestmentProducts)e.RowData;

            if (InvestmentProducts.description == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("descrìption", "Tiene que rellenar la descripción");
            }

        }

        private void gvInvestmentProducts_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            InvestmentProducts InvestmentProducts = (InvestmentProducts)e.RowData;
            servicesContainer.GetInstance<InvestmentProductsService>().update(InvestmentProducts);
        }

        private void gvInvestmentProducts_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (InvestmentProducts InvestmentProducts in e.Items)
            {
                servicesContainer.GetInstance<InvestmentProductsService>().delete(InvestmentProducts);
            }
        }

        private void gvInvestmentProducts_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer eliminar este tag?", "Eliminación tag", MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            gvInvestmentProducts.SearchHelper.AllowFiltering = true;
            gvInvestmentProducts.SearchHelper.Search(txtSearch.Text);
        }
    }
}
