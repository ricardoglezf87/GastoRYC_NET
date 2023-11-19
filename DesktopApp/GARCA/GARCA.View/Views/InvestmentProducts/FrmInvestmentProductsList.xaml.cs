using GARCA.Models;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static GARCA.Data.IOC.DependencyConfig;

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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbInvestmentProductsTypes.ItemsSource = await iInvestmentProductsTypesService.GetAll();
            await LoadItemSource();
        }

        private async Task LoadItemSource()
        {
            gvInvestmentProducts.ItemsSource = await iInvestmentProductsService.GetAll();
        }

        private void gvInvestmentProducts_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            var investmentProducts = (InvestmentProducts)e.RowData;

            if (string.IsNullOrWhiteSpace(investmentProducts.Description))
            {
                e.IsValid = false;
                e.ErrorMessages.Add("Descrìption", "Tiene que rellenar la descripción");
            }

            if (investmentProducts.InvestmentProductsTypesId == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("InvestmentProductsTypesid", "Tiene que rellenar el tipo del producto financiero");
            }
        }

        private async void gvInvestmentProducts_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            var investmentProducts = (InvestmentProducts)gvInvestmentProducts.SelectedItem;
            if (investmentProducts != null)
            {
                switch (gvInvestmentProducts.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "investmentProductsTypesId":
                        investmentProducts.InvestmentProductsTypes = await iInvestmentProductsTypesService.GetById(investmentProducts.InvestmentProductsTypesId ?? -99);
                        break;
                }
            }
        }

        private async void gvInvestmentProducts_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var investmentProducts = (InvestmentProducts)e.RowData;

            if (investmentProducts.InvestmentProductsTypes == null && investmentProducts.InvestmentProductsTypesId != null)
            {
                investmentProducts.InvestmentProductsTypes = await iInvestmentProductsTypesService.GetById(investmentProducts.InvestmentProductsTypesId ?? -99);
            }

            await iInvestmentProductsService.Save(investmentProducts);
            await LoadItemSource();
        }

        private async void gvInvestmentProducts_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (InvestmentProducts investmentProducts in e.Items)
            {
                await iInvestmentProductsService.Delete(investmentProducts);
            }
            await LoadItemSource();
        }

        private void gvInvestmentProducts_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer eliminar este producto de inversión?", "Eliminación producto de inversión", MessageBoxButton.YesNo,
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

        private void gvInvestmentProducts_CurrentCellValueChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellValueChangedEventArgs e)
        {
            int columnIndex = this.gvInvestmentProducts.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex);

            if (this.gvInvestmentProducts.Columns[columnIndex].CellType == "CheckBox")
            {
                this.gvInvestmentProducts.GetValidationHelper().SetCurrentRowValidated(false);
            }
        }
    }
}
