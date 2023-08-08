using GARCA.BO.Models;
using GARCA.BO.Services;
using GARCA.Utils.IOC;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

//TODO: En esta version de syncfusion no permite guardar los checkbox al perder foco, tienes que saltar a un texbox antes de saltar de linea

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
            cbAccountsTypes.ItemsSource = DependencyConfig.AccountsTypesService.GetAll();
            gvAccounts.ItemsSource = DependencyConfig.AccountsService.GetAll()?.ToList();
        }

        private void gvAccounts_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            var accounts = (Accounts)gvAccounts.SelectedItem;
            if (accounts != null)
            {
                switch (gvAccounts.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "accountsTypesid":
                        accounts.AccountsTypes = DependencyConfig.AccountsTypesService.GetById(accounts.AccountsTypesid);
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
                categories = DependencyConfig.CategoriesService.GetById(accounts.Categoryid);
                if (categories != null)
                {
                    categories.Description = "[" + accounts.Description + "]";
                    DependencyConfig.CategoriesService.Update(categories);
                }
            }
            else
            {
                categories = new Categories();
                accounts.Categoryid = DependencyConfig.CategoriesService.GetNextId();
                categories.Description = "[" + accounts.Description + "]";
                categories.CategoriesTypesid = (int)CategoriesTypesService.ECategoriesTypes.Transfers;

            }

            if (categories != null)
            {
                DependencyConfig.CategoriesService.Update(categories);
            }
        }

        private void gvAccounts_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var accounts = (Accounts)e.RowData;

            if (accounts.AccountsTypes == null && accounts.AccountsTypesid != null)
            {
                accounts.AccountsTypes = DependencyConfig.AccountsTypesService.GetById(accounts.AccountsTypesid);
            }

            UpdateCategory(accounts);
            DependencyConfig.AccountsService.Update(accounts);
        }

        private void gvAccounts_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Accounts accounts in e.Items)
            {
                var categories = DependencyConfig.CategoriesService.GetById(accounts.Categoryid);
                if (categories != null)
                {
                    DependencyConfig.CategoriesService.Delete(categories);
                }

                DependencyConfig.AccountsService.Delete(accounts);
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
    }
}
