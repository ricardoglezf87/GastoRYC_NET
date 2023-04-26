using BBDDLib.Models;
using GastosRYC.BBDDLib.Services;
using System.Windows;
using System.Windows.Controls;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para TransactionsReminders.xaml
    /// </summary>
    public partial class FrmTransactionReminderList : Window
    {

        private readonly SimpleInjector.Container servicesContainer;

        public FrmTransactionReminderList(SimpleInjector.Container servicesContainer)
        {
            InitializeComponent();
            this.servicesContainer = servicesContainer;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadTransactions();
        }

        private void gvTransactionsReminders_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (TransactionsReminders transactionsReminders in e.Items)
            {
                servicesContainer.GetInstance<TransactionsRemindersService>().delete(transactionsReminders);
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
                FrmTransactionReminders frm = new FrmTransactionReminders((TransactionsReminders)gvTransactionsReminders.CurrentItem, servicesContainer);
                frm.ShowDialog();
                loadTransactions();
            }
        }

        private void loadTransactions()
        {
            gvTransactionsReminders.ItemsSource = servicesContainer.GetInstance<TransactionsRemindersService>().getAll();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            FrmTransactionReminders frm = new FrmTransactionReminders(servicesContainer);
            frm.ShowDialog();
            loadTransactions();
        }
    }
}
