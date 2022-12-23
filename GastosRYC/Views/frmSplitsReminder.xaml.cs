using BBDDLib.Models;
using GastosRYC.BBDDLib.Services;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para SplitsReminder.xaml
    /// </summary>
    public partial class frmSplitsReminder : Window
    {

        private readonly CategoriesService categoriesService = new CategoriesService();
        private readonly SplitsReminderService splitsReminderService = new SplitsReminderService();
        private readonly TransactionsReminderService transactionsReminderService = new TransactionsReminderService();
        private readonly TransactionsReminder? transactionsReminder;

        public frmSplitsReminder(TransactionsReminder? transactionsReminder)
        {
            InitializeComponent();
            this.transactionsReminder = transactionsReminder;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbCategories.ItemsSource = categoriesService.getAll();
            if (transactionsReminder != null && transactionsReminder.id > 0)
            {
                gvSplitsReminder.ItemsSource = splitsReminderService.getbyTransactionid(transactionsReminder.id);
            }
            else
            {
                gvSplitsReminder.ItemsSource = splitsReminderService.getbyTransactionidNull();
            }
        }

        private void gvSplitsReminder_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            SplitsReminder splitsReminder = (SplitsReminder)gvSplitsReminder.SelectedItem;
            if (splitsReminder != null)
            {
                switch (gvSplitsReminder.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "categoryid":
                        splitsReminder.category = categoriesService.getByID(splitsReminder.categoryid);
                        break;                    
                }
            }
        }

       
        private void gvSplitsReminder_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            SplitsReminder splitsReminder = (SplitsReminder)e.RowData;
            
            if (splitsReminder.categoryid == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("categoryid", "Tiene que rellenar el tipo de categoría");
            }
            else if(splitsReminder.categoryid == (int)CategoriesService.eSpecialCategories.Split)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("categoryid", "No se puede utilizar esta categoría en un split");
            }

            if (splitsReminder.amountIn == null && splitsReminder.amountOut == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("amountIn", "Tiene que rellenar la cantidad");
                e.ErrorMessages.Add("amountOut", "Tiene que rellenar la cantidad");
            }
        }                
        
        private void gvSplitsReminder_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            SplitsReminder splitsReminder = (SplitsReminder)e.RowData;

            saveChanges(splitsReminder);
        }

        private void saveChanges(SplitsReminder splitsReminder)
        {
            if (splitsReminder.category == null && splitsReminder.categoryid != null)
            {
                splitsReminder.category = categoriesService.getByID(splitsReminder.categoryid);
            }

            if (splitsReminder.amountIn == null)
                splitsReminder.amountIn = 0;

            if (splitsReminder.amountOut == null)
                splitsReminder.amountOut = 0;

            updateTranfer(splitsReminder);

            splitsReminderService.update(splitsReminder);
        }

        private void updateTranfer(SplitsReminder splitsReminder)
        {
            if (splitsReminder.tranferid != null && 
                splitsReminder.category.categoriesTypesid != (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                TransactionsReminder? tContraria = transactionsReminderService.getByID(splitsReminder.tranferid);
                if (tContraria != null)
                {
                    transactionsReminderService.delete(tContraria);
                }
                splitsReminder.tranferid = null;
            }
            else if (splitsReminder.tranferid == null && 
                splitsReminder.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                splitsReminder.tranferid = transactionsReminderService.getNextID();

                TransactionsReminder? tContraria = new TransactionsReminder();
                tContraria.date = transactionsReminder.date;
                tContraria.accountid = splitsReminder.category.accounts.id;
                tContraria.personid = transactionsReminder.personid;
                tContraria.categoryid = transactionsReminder.account.categoryid;
                tContraria.memo = splitsReminder.memo;
                tContraria.tagid = transactionsReminder.tagid;
                tContraria.amountIn = splitsReminder.amountOut;
                tContraria.amountOut = splitsReminder.amountIn;

                if (splitsReminder.id != 0)
                    tContraria.tranferSplitid = splitsReminder.id;
                else
                    tContraria.tranferSplitid = splitsReminderService.getNextID() + 1;

                tContraria.transactionStatusid = transactionsReminder.transactionStatusid;

                transactionsReminderService.update(tContraria);

            }
            else if (splitsReminder.tranferid != null && 
                splitsReminder.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            {
                TransactionsReminder? tContraria = transactionsReminderService.getByID(splitsReminder.tranferid);
                if (tContraria != null)
                {
                    tContraria.date = transactionsReminder.date;
                    tContraria.accountid = splitsReminder.category.accounts.id;
                    tContraria.personid = transactionsReminder.personid;
                    tContraria.categoryid = transactionsReminder.account.categoryid;
                    tContraria.memo = splitsReminder.memo;
                    tContraria.tagid = transactionsReminder.tagid;
                    tContraria.amountIn = splitsReminder.amountOut??0;
                    tContraria.amountOut = splitsReminder.amountIn??0;
                    tContraria.transactionStatusid = transactionsReminder.transactionStatusid;
                    transactionsReminderService.update(tContraria);
                }
            }
        }


        private void gvSplitsReminder_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (SplitsReminder splitsReminder in e.Items) {
                if (splitsReminder.tranferid != null)
                {
                    transactionsReminderService.delete(transactionsReminderService.getByID(splitsReminder.tranferid));
                }
                splitsReminderService.delete(splitsReminder);
            }            
        }

        private void gvSplitsReminder_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if(MessageBox.Show("Esta seguro de querer eliminar esta split?","Eliminación split",MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void gvSplitsReminder_AddNewRowInitiating(object sender, Syncfusion.UI.Xaml.Grid.AddNewRowInitiatingEventArgs e)
        {
            SplitsReminder splitsReminder = (SplitsReminder)e.NewObject;
            splitsReminder.transactionid = transactionsReminder.id;
            splitsReminder.transaction = transactionsReminder;
        }
    }
}
