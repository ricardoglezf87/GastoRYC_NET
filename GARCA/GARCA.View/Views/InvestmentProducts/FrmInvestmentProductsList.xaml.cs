using GARCA.BO.Models;
using GARCA.Utils.IOC;
using System.Windows;
using System.Windows.Controls;

//TODO: En esta version de syncfusion no permite guardar los checkbox al perder foco, tienes que saltar a un texbox antes de saltar de linea

namespace GARCA.View.Views
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
            cbInvestmentProductsTypes.ItemsSource = DependencyConfig.IInvestmentProductsTypesService.GetAll();
            gvInvestmentProducts.ItemsSource = DependencyConfig.IInvestmentProductsService.GetAll();
        }

        private void gvInvestmentProducts_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            var investmentProducts = (InvestmentProducts)e.RowData;

            if (investmentProducts.Description == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("descrìption", "Tiene que rellenar la descripción");
            }

            if (investmentProducts.InvestmentProductsTypesid == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("investmentProductsTypesid", "Tiene que rellenar el tipo del producto financiero");
            }
        }

        private void gvInvestmentProducts_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            var investmentProducts = (InvestmentProducts)gvInvestmentProducts.SelectedItem;
            if (investmentProducts != null)
            {
                switch (gvInvestmentProducts.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "investmentProductsTypesid":
                        investmentProducts.InvestmentProductsTypes = DependencyConfig.IInvestmentProductsTypesService.GetById(investmentProducts.InvestmentProductsTypesid);
                        break;
                }
            }
        }

        private void gvInvestmentProducts_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var investmentProducts = (InvestmentProducts)e.RowData;

            if (investmentProducts.InvestmentProductsTypes == null && investmentProducts.InvestmentProductsTypesid != null)
            {
                investmentProducts.InvestmentProductsTypes = DependencyConfig.IInvestmentProductsTypesService.GetById(investmentProducts.InvestmentProductsTypesid);
            }

            DependencyConfig.IInvestmentProductsService.Update(investmentProducts);
        }

        private void gvInvestmentProducts_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (InvestmentProducts investmentProducts in e.Items)
            {
                DependencyConfig.IInvestmentProductsService.Delete(investmentProducts);
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
