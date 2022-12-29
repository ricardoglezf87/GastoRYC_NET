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

        private readonly TransactionsRemindersService transactionsRemindersService = new TransactionsRemindersService();

        public FrmTransactionReminderList()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {           
            gvTransactionsReminders.ItemsSource = transactionsRemindersService.getAll();            
        }       

        private void gvTransactionsReminders_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (TransactionsReminders transactionsReminders in e.Items) {                
                transactionsRemindersService.delete(transactionsReminders);
            }            
        }

        private void gvTransactionsReminders_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if(MessageBox.Show("Esta seguro de querer eliminar este tag?","Eliminación tag",MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            gvTransactionsReminders.SearchHelper.AllowFiltering = true;
            gvTransactionsReminders.SearchHelper.Search(txtSearch.Text);
        }
    }
}
