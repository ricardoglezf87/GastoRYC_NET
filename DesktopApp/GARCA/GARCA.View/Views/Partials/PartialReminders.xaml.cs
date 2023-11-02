using GARCA.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.View.Views
{
    /// <summary>
    /// Lógica de interacción para partialTransactions.xaml
    /// </summary>
    public partial class PartialReminders : Page
    {
        #region Variables

        private readonly MainWindow parentForm;

        #endregion

        #region Constructor

        public PartialReminders(MainWindow parentForm)
        {
            InitializeComponent();
            this.parentForm = parentForm;
        }

        #endregion

        #region Events

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReminders();
        }

        private void btnSkip_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer saltar este recordatorío?", "recordatorio movimiento", MessageBoxButton.YesNo,
                   MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes 
                   && ((Button)sender).Tag != null)
            {
                PutDoneReminder((int)((Button)sender).Tag);
            }
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer registrar este recordatorío?", "recordatorio movimiento", MessageBoxButton.YesNo,
               MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes
               && ((Button)sender).Tag != null)
            {
                MakeTransactionFromReminder((int)((Button)sender).Tag);
                PutDoneReminder((int)((Button)sender).Tag);
            }
        }

        private void cvReminders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (cvReminders.SelectedItem != null && ((ExpirationsReminders)cvReminders.SelectedItem).TransactionsReminders != null)
            {
                FrmTransactionReminders frm = new(((ExpirationsReminders)cvReminders.SelectedItem).TransactionsReminders);
                frm.ShowDialog();
                LoadReminders();
            }
        }

        #endregion

        #region Functions

        public async Task LoadReminders()
        {
            var expirationsReminders = await Task.Run(() => iExpirationsRemindersService.GetAllPendingWithoutFutureWithGeneration()?.ToList());

            cvReminders.ItemsSource = new ListCollectionView(expirationsReminders);

            cvReminders.CanGroup = true;
            cvReminders.GroupCards("GroupDate");

            cvReminders.Items.SortDescriptions.Clear();
            cvReminders.Items.SortDescriptions.Add(
                new System.ComponentModel.SortDescription("Date", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void PutDoneReminder(int? id)
        {
            var expirationsReminders = iExpirationsRemindersService.GetById(id);
            if (expirationsReminders != null)
            {
                expirationsReminders.Done = true;
                iExpirationsRemindersService.Update(expirationsReminders);
            }

            LoadReminders();
        }

        private async Task MakeTransactionFromReminder(int? id)
        {
            var transaction = await iExpirationsRemindersService.RegisterTransactionfromReminder(id);
            if (transaction != null)
            {
                FrmTransaction frm = new(transaction);
                frm.ShowDialog();
            }

            parentForm.LoadAccounts();
        }

        #endregion

    }
}

