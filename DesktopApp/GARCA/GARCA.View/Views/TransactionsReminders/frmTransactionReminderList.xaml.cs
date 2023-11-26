using GARCA.Models;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.View.Views
{
    /// <summary>
    /// Lógica de interacción para TransactionsReminders.xaml
    /// </summary>
    public partial class FrmTransactionReminderList : Window
    {
        public FrmTransactionReminderList()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTransactions();
        }

        private async void gvTransactionsReminders_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (TransactionsReminders transactionsReminders in e.Items)
            {
                await iTransactionsRemindersService.Delete(transactionsReminders);
            }
        }

        private void gvTransactionsReminders_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer eliminar este recordatorio?", "Eliminación recordatorio", MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            gvTransactionsReminders.SearchHelper.AllowFiltering = true;
            gvTransactionsReminders.SearchHelper.Search(txtSearch.Text);
        }

        private async void gvTransactionsReminders_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (gvTransactionsReminders.CurrentItem != null)
            {
                FrmTransactionReminders frm = new((TransactionsReminders)gvTransactionsReminders.CurrentItem);
                frm.ShowDialog();
                await LoadTransactions();
            }
        }

        private async Task LoadTransactions()
        {
            gvTransactionsReminders.ItemsSource = await iTransactionsRemindersService.GetAll();
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            FrmTransactionReminders frm = new();
            frm.ShowDialog();
            await LoadTransactions();
        }
    }
}
