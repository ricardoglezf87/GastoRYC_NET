using BBDDLib.Models;
using GastosRYC.BBDDLib.Services;
using System.Windows;
using System.Windows.Controls;

namespace GastosRYC.Views
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
            cbAccountsTypes.ItemsSource = RYCContextService.accountsTypesService.getAll();
            gvAccounts.ItemsSource = RYCContextService.accountsService.getAll();            
        }

        private void gvAccounts_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            Accounts accounts = (Accounts)gvAccounts.SelectedItem;
            if (accounts != null)
            {
                switch (gvAccounts.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "accountsTypesid":
                        accounts.accountsTypes = RYCContextService.accountsTypesService.getByID(accounts.accountsTypesid);
                        break;                    
                }
            }
        }

       
        private void gvAccounts_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            Accounts accounts = (Accounts)e.RowData;

            if (accounts.description == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("description", "Tiene que rellenar la descripción");
            }

            if (accounts.accountsTypesid == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("accountsTypesid", "Tiene que rellenar el tipo de cuenta");
            }
        } 

        private void updateCategory(Accounts accounts)
        {
            Categories? categories;

            if (accounts.categoryid != null)
            {
                categories = RYCContextService.categoriesService.getByID(accounts.categoryid);
                if(categories != null)
                {
                    categories.description = "[" + accounts.description + "]";
                    RYCContextService.categoriesService.update(categories);
                }
            }else
            {
                categories = new Categories();
                accounts.categoryid = RYCContextService.categoriesService.getNextID(); ;
                categories.description = "[" + accounts.description + "]";
                categories.categoriesTypesid = (int)CategoriesTypesService.eCategoriesTypes.Transfers;

            }

            if (categories != null)
            {
                RYCContextService.categoriesService.update(categories);
            }
        }
        
        private void gvAccounts_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            Accounts accounts = (Accounts)e.RowData;

            if (accounts.accountsTypes == null && accounts.accountsTypesid != null)
            {
                accounts.accountsTypes = RYCContextService.accountsTypesService.getByID(accounts.accountsTypesid);               
            }

            updateCategory(accounts);
            RYCContextService.accountsService.update(accounts);
        }

        private void gvAccounts_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Accounts accounts in e.Items) {
                Categories? categories = RYCContextService.categoriesService.getByID(accounts.categoryid);
                if(categories!= null)
                {
                    RYCContextService.categoriesService.delete(categories);
                }

                RYCContextService.accountsService.delete(accounts);
            }            
        }

        private void gvAccounts_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if(MessageBox.Show("Esta seguro de querer eliminar esta cuenta?","Eliminación Cuenta",MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,MessageBoxResult.No) == MessageBoxResult.No)
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
