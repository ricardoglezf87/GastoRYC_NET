using GARCA.Data.Services;
using GARCA.Models;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static GARCA.Data.IOC.DependencyConfig;
using static GARCA.Utils.Enums.EnumCategories;

namespace GARCA.View.Views
{
    /// <summary>
    /// Lógica de interacción para Cuentas.xaml
    /// </summary>
    public partial class FrmAccountsList : Window
    {
        public FrmAccountsList()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbAccountsTypes.ItemsSource = await iAccountsTypesService.GetAll();
            await LoadItemSource();
        }

        private async Task LoadItemSource()
        {
            gvAccounts.ItemsSource = (await iAccountsService.GetAll())?.ToList();
        }

        private async void gvAccounts_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            var accounts = (Accounts)gvAccounts.SelectedItem;
            if (accounts != null)
            {
                switch (gvAccounts.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "accountsTypesid":
                        accounts.AccountsTypes = await iAccountsTypesService.GetById(accounts.AccountsTypesId ?? -99);
                        break;
                }
            }
        }


        private void gvAccounts_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            var accounts = (Accounts)e.RowData;

            if (string.IsNullOrWhiteSpace(accounts.Description))
            {
                e.IsValid = false;
                e.ErrorMessages.Add("Description", "Tiene que rellenar la descripción");
            }

            if (accounts.AccountsTypesId == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("AccountsTypesId", "Tiene que rellenar el tipo de cuenta");
            }
        }

        private async void gvAccounts_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            try
            {
                var accounts = (Accounts)e.RowData;

                if (accounts.AccountsTypes == null && accounts.AccountsTypesId != null)
                {
                    accounts.AccountsTypes = await iAccountsTypesService.GetById(accounts.AccountsTypesId ?? -99);
                }

                await iAccountsService.Save(accounts);
                await LoadItemSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GARCA");
            }
        }

        private async void gvAccounts_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            try
            {
                foreach (Accounts accounts in e.Items)
                {
                    var categories = await iCategoriesService.GetById(accounts.Categoryid ?? -99);
                    if (categories != null)
                    {
                        await iCategoriesService.Delete(categories);
                    }

                    await iAccountsService.Delete(accounts);
                }
                await LoadItemSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GARCA");
            }
        }

        private void gvAccounts_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer eliminar esta cuenta?", "Eliminación Cuenta", MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            gvAccounts.SearchHelper.AllowFiltering = true;
            gvAccounts.SearchHelper.Search(txtSearch.Text);
        }

        private void gvAccounts_CurrentCellValueChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellValueChangedEventArgs e)
        {
            int columnIndex = this.gvAccounts.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex);

            if (this.gvAccounts.Columns[columnIndex].CellType == "CheckBox")
            {
                this.gvAccounts.GetValidationHelper().SetCurrentRowValidated(false);
            }
        }
    }
}
