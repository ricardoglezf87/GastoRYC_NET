using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;
using GARCA.Data.Services;
using System.Windows;

namespace GARCA.View.Views
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
            cbCategories.ItemsSource = iCategoriesService.GetAll();
            loadSplits();
        }

        private void loadSplits()
        {
            gvSplitsReminders.ItemsSource = transactionsReminders != null && transactionsReminders.Id > 0
                ? iSplitsRemindersService.GetbyTransactionid(transactionsReminders.Id)
                : (object)iSplitsRemindersService.GetbyTransactionidNull();
        }

        private void gvSplitsReminders_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            var splitsReminders = (SplitsReminders)gvSplitsReminders.SelectedItem;
            if (splitsReminders != null)
            {
                switch (gvSplitsReminders.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "categoryid":
                        splitsReminders.Category = iCategoriesService.GetById(splitsReminders.Categoryid);
                        break;
                }
            }
        }

        private void gvSplitsReminders_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            var splitsReminders = (SplitsReminders)e.RowData;

            if (splitsReminders.Categoryid == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("Categoryid", "Tiene que rellenar el tipo de categoría");
            }
            else if (splitsReminders.Categoryid == (int)CategoriesService.ESpecialCategories.Split)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("Categoryid", "No se puede utilizar esta categoría en un split");
            }

            if (splitsReminders.AmountIn == null && splitsReminders.AmountOut == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("AmountIn", "Tiene que rellenar la cantidad");
                e.ErrorMessages.Add("AmountOut", "Tiene que rellenar la cantidad");
            }
        }

        private void gvSplitsReminders_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var splitsReminders = (SplitsReminders)e.RowData;

            SaveChanges(splitsReminders);
        }

        private void SaveChanges(SplitsReminders splitsReminders)
        {
            if (splitsReminders.Category == null && splitsReminders.Categoryid != null)
            {
                splitsReminders.Category = iCategoriesService.GetById(splitsReminders.Categoryid);
            }

            splitsReminders.AmountIn ??= 0;

            splitsReminders.AmountOut ??= 0;

            iSplitsRemindersService.Update(splitsReminders);
        }
        private void gvSplitsReminders_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (SplitsReminders splitsReminders in e.Items)
            {
                if (splitsReminders.Tranferid != null)
                {
                    iTransactionsRemindersService.Delete(iTransactionsRemindersService.GetById(splitsReminders.Tranferid));
                }
                iSplitsRemindersService.Delete(splitsReminders);
            }
            loadSplits();
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
            var splitsReminders = (SplitsReminders)e.NewObject;
            splitsReminders.Transactionid = transactionsReminders.Id;
            splitsReminders.Transaction = transactionsReminders;
        }
    }
}
