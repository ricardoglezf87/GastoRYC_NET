using GARCA.Data.Services;
using GARCA.Models;
using System.Threading.Tasks;
using System.Windows;
using static GARCA.Data.IOC.DependencyConfig;
using static GARCA.Utils.Enums.EnumCategories;

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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbCategories.ItemsSource = await iCategoriesService.GetAll();
            await loadSplits();
        }

        private async Task loadSplits()
        {
            gvSplitsReminders.ItemsSource = transactionsReminders != null && transactionsReminders.Id > 0
                ? await iSplitsRemindersService.GetbyTransactionid(transactionsReminders.Id)
                : (object?)await iSplitsRemindersService.GetbyTransactionidNull();
        }

        private async void gvSplitsReminders_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            var splitsReminders = (SplitsReminders)gvSplitsReminders.SelectedItem;
            if (splitsReminders != null)
            {
                switch (gvSplitsReminders.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "CategoriesId":
                        splitsReminders.Categories = await iCategoriesService.GetById(splitsReminders.CategoriesId ?? -99);
                        break;
                }
            }
        }

        private void gvSplitsReminders_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            var splitsReminders = (SplitsReminders)e.RowData;

            if (splitsReminders.CategoriesId == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("CategoriesId", "Tiene que rellenar el tipo de categoría");
            }
            else if (splitsReminders.CategoriesId == (int)ESpecialCategories.Split)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("CategoriesId", "No se puede utilizar esta categoría en un split");
            }

            if (splitsReminders.AmountIn == null && splitsReminders.AmountOut == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("AmountIn", "Tiene que rellenar la cantidad");
                e.ErrorMessages.Add("AmountOut", "Tiene que rellenar la cantidad");
            }
        }

        private async void gvSplitsReminders_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var splitsReminders = (SplitsReminders)e.RowData;

            await SaveChanges(splitsReminders);
        }

        private async Task SaveChanges(SplitsReminders splitsReminders)
        {
            if (splitsReminders.Categories == null && splitsReminders.CategoriesId != null)
            {
                splitsReminders.Categories = await iCategoriesService.GetById(splitsReminders.CategoriesId ?? -99);
            }

            splitsReminders.AmountIn ??= 0;

            splitsReminders.AmountOut ??= 0;

            await iSplitsRemindersService.Save(splitsReminders);
        }
        private async void gvSplitsReminders_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (SplitsReminders splitsReminders in e.Items)
            {
                if (splitsReminders.TranferId != null)
                {
                    await iTransactionsRemindersService.Delete(await iTransactionsRemindersService.GetById(splitsReminders.TranferId ?? -99));
                }
                await iSplitsRemindersService.Delete(splitsReminders);
            }
            await loadSplits();
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
            splitsReminders.TransactionsId = transactionsReminders.Id;
            splitsReminders.Transactions = transactionsReminders;
        }
    }
}
