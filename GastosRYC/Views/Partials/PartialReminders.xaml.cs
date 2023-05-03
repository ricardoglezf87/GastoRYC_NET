using BBDDLib.Models;
using GastosRYC.BBDDLib.Services;
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

        private readonly SimpleInjector.Container servicesContainer;
        private readonly MainWindow parentForm;

        #endregion

        #region Constructor

        public PartialReminders(SimpleInjector.Container _servicesContainer, MainWindow _parentForm)
        {
            InitializeComponent();
            servicesContainer = _servicesContainer;
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
                FrmTransactionReminders frm = new FrmTransactionReminders(((ExpirationsReminders)cvReminders.SelectedItem).transactionsReminders, servicesContainer);
                frm.ShowDialog();
                loadReminders();
            }
        }

        #endregion


        #region Functions

        public void loadReminders()
        {
            servicesContainer.GetInstance<ExpirationsRemindersService>().generateAutoregister();
            parentForm.loadAccounts();

            cvReminders.ItemsSource = new ListCollectionView(servicesContainer.GetInstance<ExpirationsRemindersService>().getAllPendingWithoutFutureWithGeneration());

            cvReminders.CanGroup = true;
            cvReminders.GroupCards("groupDate");

            cvReminders.Items.SortDescriptions.Clear();
            cvReminders.Items.SortDescriptions.Add(
                new System.ComponentModel.SortDescription("date", System.ComponentModel.ListSortDirection.Ascending));
        }

        private void putDoneReminder(int? id)
        {
            ExpirationsReminders? expirationsReminders = servicesContainer.GetInstance<ExpirationsRemindersService>().getByID(id);
            if (expirationsReminders != null)
            {
                expirationsReminders.done = true;
                servicesContainer.GetInstance<ExpirationsRemindersService>().update(expirationsReminders);
            }

            loadReminders();
        }

        private void makeTransactionFromReminder(int? id)
        {
            Transactions? transaction =  servicesContainer.GetInstance<ExpirationsRemindersService>().registerTransactionfromReminder(id);
            if(transaction != null)
            {
                FrmTransaction frm = new FrmTransaction(transaction, servicesContainer);
            }

            parentForm.loadAccounts();
        }

        #endregion

    }
}

