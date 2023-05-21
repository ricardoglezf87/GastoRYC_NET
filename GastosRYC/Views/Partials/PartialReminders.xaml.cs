using BOLib.Models;
using BOLib.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace GastosRYC.Views
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

        public PartialReminders(MainWindow _parentForm)
        {
            InitializeComponent();
            parentForm = _parentForm;
        }

        #endregion

        #region Events

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadReminders();
        }

        private void btnSkip_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer saltar este recordatorío?", "recordatorio movimiento", MessageBoxButton.YesNo,
                   MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                if (sender != null && ((Button)sender)?.Tag != null)
                {
                    putDoneReminder((int?)((Button)sender).Tag);
                }
            }
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer registrar este recordatorío?", "recordatorio movimiento", MessageBoxButton.YesNo,
               MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                if (sender != null && ((Button)sender)?.Tag != null)
                {
                    makeTransactionFromReminder((int?)((Button)sender).Tag);
                    putDoneReminder((int?)((Button)sender).Tag);
                }
            }
        }

        private void cvReminders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (cvReminders.SelectedItem != null && ((ExpirationsReminders)cvReminders.SelectedItem).transactionsReminders != null)
            {
                FrmTransactionReminders frm = new(((ExpirationsReminders)cvReminders.SelectedItem).transactionsReminders);
                frm.ShowDialog();
                loadReminders();
            }
        }

        #endregion

        #region Functions

        public async void loadReminders()
        {
            await Task.Run(() => ExpirationsRemindersService.Instance.generateAutoregister());
            parentForm.loadAccounts();

            List<ExpirationsReminders?>? expirationsReminders = await Task.Run(() => ExpirationsRemindersService.Instance.getAllPendingWithoutFutureWithGeneration());

            cvReminders.ItemsSource = new ListCollectionView(expirationsReminders);

            cvReminders.CanGroup = true;
            cvReminders.GroupCards("groupDate");

            cvReminders.Items.SortDescriptions.Clear();
            cvReminders.Items.SortDescriptions.Add(
                new System.ComponentModel.SortDescription("date", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void putDoneReminder(int? id)
        {
            ExpirationsReminders? expirationsReminders = ExpirationsRemindersService.Instance.getByID(id);
            if (expirationsReminders != null)
            {
                expirationsReminders.done = true;
                ExpirationsRemindersService.Instance.update(expirationsReminders);
            }

            loadReminders();
        }

        private void makeTransactionFromReminder(int? id)
        {
            Transactions? transaction = ExpirationsRemindersService.Instance.registerTransactionfromReminder(id);
            if (transaction != null)
            {
                FrmTransaction frm = new(transaction);
            }

            parentForm.loadAccounts();
        }

        #endregion

    }
}

