using GARCA.Data.Services;
using GARCA.Models;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static GARCA.Data.IOC.DependencyConfig;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbAccountsTypes.ItemsSource = iAccountsTypesService.GetAll();
            LoadItemSource();
        }

        private void LoadItemSource()
        {
            gvAccounts.ItemsSource = iAccountsService.GetAll()?.ToList();
        }

        private void gvAccounts_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            var accounts = (Accounts)gvAccounts.SelectedItem;
            if (accounts != null)
            {
                switch (gvAccounts.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "accountsTypesid":
                        accounts.AccountsTypes = iAccountsTypesService.GetById(accounts.AccountsTypesid ?? -99);
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

            if (accounts.AccountsTypesid == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("AccountsTypesid", "Tiene que rellenar el tipo de cuenta");
            }
        }

        private void UpdateCategory(Accounts accounts)
        {
            Categories? categories;

            if (accounts.Categoryid != null)
            {
                categories = iCategoriesService.GetById(accounts.Categoryid ?? -99);
                if (categories != null)
                {
                    categories.Description = "[" + accounts.Description + "]";
                    iCategoriesService.Update(categories);
                }
            }
            else
            {
                categories = new Categories();
                accounts.Categoryid = iCategoriesService.GetNextId();
                categories.Description = "[" + accounts.Description + "]";
                categories.CategoriesTypesid = (int)CategoriesTypesService.ECategoriesTypes.Transfers;

            }

            if (categories != null)
            {
                iCategoriesService.Update(categories);
            }
        }

        private void gvAccounts_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var accounts = (Accounts)e.RowData;

            if (accounts.AccountsTypes == null && accounts.AccountsTypesid != null)
            {
                accounts.AccountsTypes = iAccountsTypesService.GetById(accounts.AccountsTypesid ?? -99);
            }

            UpdateCategory(accounts);
            iAccountsService.Update(accounts);
            LoadItemSource();
        }

        private void gvAccounts_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Accounts accounts in e.Items)
            {
                var categories = iCategoriesService.GetById(accounts.Categoryid ?? -99);
                if (categories != null)
                {
                    iCategoriesService.Delete(categories);
                }

                iAccountsService.Delete(accounts);
            }
            LoadItemSource();
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
