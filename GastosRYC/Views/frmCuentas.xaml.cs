using BBDDLib.Models;
using GastosRYC.BBDDLib.Services;
using System.Windows;
using System.Windows.Controls;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para Cuentas.xaml
    /// </summary>
    public partial class frmCuentas : Window
    {

        private readonly AccountsService accountsService = new AccountsService();
        private readonly AccountsTypesService accountsTypesService = new AccountsTypesService();
        private readonly CategoriesService categoriesService = new CategoriesService();

        public frmCuentas()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbAccountsTypes.ItemsSource = accountsTypesService.getAll();
            gvCuentas.ItemsSource = accountsService.getAll();            
        }

        private void gvCuentas_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            Accounts accounts = (Accounts)gvCuentas.SelectedItem;
            if (accounts != null)
            {
                switch (gvCuentas.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "accountsTypesid":
                        accounts.accountsTypes = accountsTypesService.getByID(accounts.accountsTypesid);
                        break;                    
                }
            }
        }

       
        private void gvCuentas_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
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
                categories = categoriesService.getByID(accounts.categoryid);
                if(categories != null)
                {
                    categories.description = "[" + accounts.description + "]";
                    categoriesService.update(categories);
                }
            }else
            {
                categories = new Categories();
                accounts.categoryid = categoriesService.getNextID(); ;
                categories.description = "[" + accounts.description + "]";
                categories.categoriesTypesid = 3;

            }

            if (categories != null)
            {
                categoriesService.update(categories);
            }
        }
        
        private void gvCuentas_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            Accounts accounts = (Accounts)e.RowData;

            if (accounts.accountsTypes == null && accounts.accountsTypesid != null)
            {
                accounts.accountsTypes = accountsTypesService.getByID(accounts.accountsTypesid);               
            }

            updateCategory(accounts);
            accountsService.update(accounts);
        }

        private void gvCuentas_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Accounts accounts in e.Items) {
                Categories? categories = categoriesService.getByID(accounts.categoryid);
                if(categories!= null)
                {
                    categoriesService.delete(categories);
                }

                accountsService.delete(accounts);
            }            
        }

        private void gvCuentas_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if(MessageBox.Show("Esta seguro de querer eliminar esta cuenta?","Eliminación Cuenta",MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            gvCuentas.SearchHelper.AllowFiltering = true;
            gvCuentas.SearchHelper.Search(txtSearch.Text);
        }
    }
}
