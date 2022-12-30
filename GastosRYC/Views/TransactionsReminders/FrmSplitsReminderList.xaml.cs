using BBDDLib.Models;
using GastosRYC.BBDDLib.Services;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para SplitsReminders.xaml
    /// </summary>
    public partial class FrmSplitsRemindersList : Window
    {

        private readonly CategoriesService categoriesService = new CategoriesService();
        private readonly SplitsRemindersService splitsRemindersService = new SplitsRemindersService();
        private readonly TransactionsRemindersService transactionsRemindersService = new TransactionsRemindersService();
        private readonly TransactionsReminders? transactionsReminders;

        public FrmSplitsRemindersList(TransactionsReminders? transactionsReminders)
        {
            InitializeComponent();
            this.transactionsReminders = transactionsReminders;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbCategories.ItemsSource = categoriesService.getAll();
            if (transactionsReminders != null && transactionsReminders.id > 0)
            {
                gvSplitsReminders.ItemsSource = splitsRemindersService.getbyTransactionid(transactionsReminders.id);
            }
            else
            {
                gvSplitsReminders.ItemsSource = splitsRemindersService.getbyTransactionidNull();
            }
        }

        private void gvSplitsReminders_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            SplitsReminders splitsReminders = (SplitsReminders)gvSplitsReminders.SelectedItem;
            if (splitsReminders != null)
            {
                switch (gvSplitsReminders.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "categoryid":
                        splitsReminders.category = categoriesService.getByID(splitsReminders.categoryid);
                        break;                    
                }
            }
        }

        private void gvSplitsReminders_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            SplitsReminders splitsReminders = (SplitsReminders)e.RowData;
            
            if (splitsReminders.categoryid == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("categoryid", "Tiene que rellenar el tipo de categoría");
            }
            else if(splitsReminders.categoryid == (int)CategoriesService.eSpecialCategories.Split)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("categoryid", "No se puede utilizar esta categoría en un split");
            }

            if (splitsReminders.amountIn == null && splitsReminders.amountOut == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("amountIn", "Tiene que rellenar la cantidad");
                e.ErrorMessages.Add("amountOut", "Tiene que rellenar la cantidad");
            }
        }                
        
        private void gvSplitsReminders_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            SplitsReminders splitsReminders = (SplitsReminders)e.RowData;

            saveChanges(splitsReminders);
        }

        private void saveChanges(SplitsReminders splitsReminders)
        {
            if (splitsReminders.category == null && splitsReminders.categoryid != null)
            {
                splitsReminders.category = categoriesService.getByID(splitsReminders.categoryid);
            }

            if (splitsReminders.amountIn == null)
                splitsReminders.amountIn = 0;

            if (splitsReminders.amountOut == null)
                splitsReminders.amountOut = 0;

            splitsRemindersService.update(splitsReminders);
        }
        private void gvSplitsReminders_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (SplitsReminders splitsReminders in e.Items) {
                if (splitsReminders.tranferid != null)
                {
                    transactionsRemindersService.delete(transactionsRemindersService.getByID(splitsReminders.tranferid));
                }
                splitsRemindersService.delete(splitsReminders);
            }            
        }

        private void gvSplitsReminders_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if(MessageBox.Show("Esta seguro de querer eliminar esta split?","Eliminación split",MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void gvSplitsReminders_AddNewRowInitiating(object sender, Syncfusion.UI.Xaml.Grid.AddNewRowInitiatingEventArgs e)
        {
            SplitsReminders splitsReminders = (SplitsReminders)e.NewObject;
            splitsReminders.transactionid = transactionsReminders.id;
            splitsReminders.transaction = transactionsReminders;
        }
    }
}
