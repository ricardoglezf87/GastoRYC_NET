using GARCA.BO.Models;
using GARCA.BO.Services;
using System.Windows;
using System.Windows.Controls;
using GARCA.Utils.IOC;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTransactions();
        }

        private void gvTransactionsReminders_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (TransactionsReminders transactionsReminders in e.Items)
            {
                DependencyConfig.ITransactionsRemindersService.Delete(transactionsReminders);
            }
        }

        private void gvTransactionsReminders_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer eliminar este tag?", "Eliminación tag", MessageBoxButton.YesNo,
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

        private void gvTransactionsReminders_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (gvTransactionsReminders.CurrentItem != null)
            {
                FrmTransactionReminders frm = new((TransactionsReminders)gvTransactionsReminders.CurrentItem);
                frm.ShowDialog();
                LoadTransactions();
            }
        }

        private void LoadTransactions()
        {
            gvTransactionsReminders.ItemsSource = DependencyConfig.ITransactionsRemindersService.GetAll();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            FrmTransactionReminders frm = new();
            frm.ShowDialog();
            LoadTransactions();
        }
    }
}
