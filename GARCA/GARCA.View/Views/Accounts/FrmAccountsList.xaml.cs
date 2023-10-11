using GARCA.BO.Models;
using GARCA.Utils.IOC;
using GARCA.View.Services;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            cbAccountsTypes.ItemsSource = DependencyConfigView.AccountsTypesServiceView.GetAll();
            LoadItemSource();
        }

        private void LoadItemSource()
        {
            gvAccounts.ItemsSource = DependencyConfigView.AccountsServiceView.GetAll()?.ToList();
        }

        private void gvAccounts_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            var accounts = (Accounts)gvAccounts.SelectedItem;
            if (accounts != null)
            {
                switch (gvAccounts.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "accountsTypesid":
                        accounts.AccountsTypes = DependencyConfigView.AccountsTypesServiceView.GetById(accounts.AccountsTypesid);
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
                categories = DependencyConfigView.CategoriesServiceView.GetById(accounts.Categoryid);
                if (categories != null)
                {
                    categories.Description = "[" + accounts.Description + "]";
                    DependencyConfigView.CategoriesServiceView.Update(categories);
                }
            }
            else
            {
                categories = new Categories();
                accounts.Categoryid = DependencyConfigView.CategoriesServiceView.GetNextId();
                categories.Description = "[" + accounts.Description + "]";
                categories.CategoriesTypesid = (int)CategoriesTypesServiceView.ECategoriesTypes.Transfers;

            }

            if (categories != null)
            {
                DependencyConfigView.CategoriesServiceView.Update(categories);
            }
        }

        private void gvAccounts_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var accounts = (Accounts)e.RowData;

            if (accounts.AccountsTypes == null && accounts.AccountsTypesid != null)
            {
                accounts.AccountsTypes = DependencyConfigView.AccountsTypesServiceView.GetById(accounts.AccountsTypesid);
            }

            UpdateCategory(accounts);
            DependencyConfigView.AccountsServiceView.Update(accounts);
            LoadItemSource();
        }

        private void gvAccounts_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Accounts accounts in e.Items)
            {
                var categories = DependencyConfigView.CategoriesServiceView.GetById(accounts.Categoryid);
                if (categories != null)
                {
                    DependencyConfigView.CategoriesServiceView.Delete(categories);
                }

                DependencyConfigView.AccountsServiceView.Delete(accounts);
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
