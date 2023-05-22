using BOLib.Models;
using BOLib.Services;
using System.Windows;
using System.Windows.Controls;

//TODO: En esta version de syncfusion no permite guardar los checkbox al perder foco, tienes que saltar a un texbox antes de saltar de linea

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para InvestmentProducts.xaml
    /// </summary>
    public partial class FrmInvestmentProductsList : Window
    {

        public FrmInvestmentProductsList()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gvInvestmentProducts.ItemsSource = InvestmentProductsService.Instance.getAll();
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
            InvestmentProductsService.Instance.update(InvestmentProducts);
        }

        private void gvInvestmentProducts_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (InvestmentProducts InvestmentProducts in e.Items)
            {
                InvestmentProductsService.Instance.delete(InvestmentProducts);
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
