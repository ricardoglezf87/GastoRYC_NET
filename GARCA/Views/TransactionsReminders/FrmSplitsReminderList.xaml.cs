using GARCA.BO.Models;
using GARCA.BO.Services;
using System.Windows;

namespace GARCA.Views
{
    /// <summary>
    /// Lógica de interacción para SplitsReminders.xaml
    /// </summary>
    public partial class FrmSplitsRemindersList : Window
    {
        private readonly TransactionsReminders? transactionsReminders;

        public FrmSplitsRemindersList()
        {
            InitializeComponent();
        }

        public FrmSplitsRemindersList(TransactionsReminders? transactionsReminders)
            : this()
        {
            this.transactionsReminders = transactionsReminders;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbCategories.ItemsSource = CategoriesService.Instance.getAll();
            gvSplitsReminders.ItemsSource = transactionsReminders != null && transactionsReminders.id > 0
                ? SplitsRemindersService.Instance.getbyTransactionid(transactionsReminders.id)
                : (object?)SplitsRemindersService.Instance.getbyTransactionidNull();
        }

        private void gvSplitsReminders_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            SplitsReminders splitsReminders = (SplitsReminders)gvSplitsReminders.SelectedItem;
            if (splitsReminders != null)
            {
                switch (gvSplitsReminders.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "categoryid":
                        splitsReminders.category = CategoriesService.Instance.getByID(splitsReminders.categoryid);
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
            else if (splitsReminders.categoryid == (int)CategoriesService.eSpecialCategories.Split)
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
                splitsReminders.category = CategoriesService.Instance.getByID(splitsReminders.categoryid);
            }

            splitsReminders.amountIn ??= 0;

            splitsReminders.amountOut ??= 0;

            SplitsRemindersService.Instance.update(splitsReminders);
        }
        private void gvSplitsReminders_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (SplitsReminders splitsReminders in e.Items)
            {
                if (splitsReminders.tranferid != null)
                {
                    TransactionsRemindersService.Instance.delete(TransactionsRemindersService.Instance.getByID(splitsReminders.tranferid));
                }
                SplitsRemindersService.Instance.delete(splitsReminders);
            }
        }

        private void gvSplitsReminders_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer eliminar esta split?", "Eliminación split", MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
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
