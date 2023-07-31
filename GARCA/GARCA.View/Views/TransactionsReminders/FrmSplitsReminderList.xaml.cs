using GARCA.BO.Models;
using GARCA.BO.Services;
using GARCA.Utils.IOC;
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
            cbCategories.ItemsSource = DependencyConfig.CategoriesService.GetAll();
            gvSplitsReminders.ItemsSource = transactionsReminders != null && transactionsReminders.Id > 0
                ? DependencyConfig.SplitsRemindersService.GetbyTransactionid(transactionsReminders.Id)
                : (object?)DependencyConfig.SplitsRemindersService.GetbyTransactionidNull();
        }

        private void gvSplitsReminders_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            var splitsReminders = (SplitsReminders)gvSplitsReminders.SelectedItem;
            if (splitsReminders != null)
            {
                switch (gvSplitsReminders.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "categoryid":
                        splitsReminders.Category = DependencyConfig.CategoriesService.GetById(splitsReminders.Categoryid);
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
                e.ErrorMessages.Add("categoryid", "Tiene que rellenar el tipo de categoría");
            }
            else if (splitsReminders.Categoryid == (int)CategoriesService.ESpecialCategories.Split)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("categoryid", "No se puede utilizar esta categoría en un split");
            }

            if (splitsReminders.AmountIn == null && splitsReminders.AmountOut == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("amountIn", "Tiene que rellenar la cantidad");
                e.ErrorMessages.Add("amountOut", "Tiene que rellenar la cantidad");
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
                splitsReminders.Category = DependencyConfig.CategoriesService.GetById(splitsReminders.Categoryid);
            }

            splitsReminders.AmountIn ??= 0;

            splitsReminders.AmountOut ??= 0;

            DependencyConfig.SplitsRemindersService.Update(splitsReminders);
        }
        private void gvSplitsReminders_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (SplitsReminders splitsReminders in e.Items)
            {
                if (splitsReminders.Tranferid != null)
                {
                    DependencyConfig.TransactionsRemindersService.Delete(DependencyConfig.TransactionsRemindersService.GetById(splitsReminders.Tranferid));
                }
                DependencyConfig.SplitsRemindersService.Delete(splitsReminders);
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
            var splitsReminders = (SplitsReminders)e.NewObject;
            splitsReminders.Transactionid = transactionsReminders.Id;
            splitsReminders.Transaction = transactionsReminders;
        }
    }
}
