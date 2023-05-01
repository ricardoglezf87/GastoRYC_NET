using BBDDLib.Models;
using GastosRYC.BBDDLib.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para FrmTransaction.xaml
    /// </summary>
    public partial class FrmTransactionReminders : Window
    {
        #region Variables
        
        private TransactionsReminders? transaction;
        private readonly SimpleInjector.Container servicesContainer;
        private readonly int? accountidDefault;

        #endregion

        #region Constructors

        public FrmTransactionReminders(SimpleInjector.Container servicesContainer)
        {
            InitializeComponent();
            this.servicesContainer = servicesContainer;
        }

        public FrmTransactionReminders(TransactionsReminders transaction, int accountidDefault, SimpleInjector.Container servicesContainer) :
            this(servicesContainer)
        {
            this.transaction = transaction;
            this.accountidDefault = accountidDefault;
        }

        public FrmTransactionReminders(TransactionsReminders? transaction, SimpleInjector.Container servicesContainer) :
            this(servicesContainer)
        {
            this.transaction = transaction;
        }

        public FrmTransactionReminders(int accountidDefault, SimpleInjector.Container servicesContainer) :
            this(servicesContainer)
        {
            this.accountidDefault = accountidDefault;
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadComboBox();
            loadTransaction();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (saveTransaction())
            {
                this.Close();
            }
        }

        private void btnSplit_Click(object sender, RoutedEventArgs e)
        {
            if (transaction == null && !saveTransaction())
            {
                MessageBox.Show("Sin guardar no se puede realizar un split", "inserción movimiento");
                return;
            }

            FrmSplitsRemindersList frm = new FrmSplitsRemindersList(transaction, servicesContainer);
            frm.ShowDialog();
            servicesContainer.GetInstance<TransactionsRemindersService>().updateSplitsReminders(transaction);
            loadTransaction();
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    if (saveTransaction())
                    {
                        transaction = null;
                        loadTransaction();
                    }
                    break;
                case Key.F2:
                    saveTransaction();
                    break;
                case Key.Escape:
                    this.Close();
                    break;
            }
        }

        private void dtpDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                if (dtpDate.Text.Length == 4)
                {
                    dtpDate.Text = dtpDate.Text.Substring(0, 2) + "/" + dtpDate.Text.Substring(2, 2);
                }
                else if (dtpDate.Text.Length == 6)
                {
                    dtpDate.Text = dtpDate.Text.Substring(0, 2) + "/" + dtpDate.Text.Substring(2, 2) + "/" + dtpDate.Text.Substring(4, 2);
                }
            }
        }

        #endregion

        #region Functions

        private void loadTransaction()
        {
            if (transaction != null)
            {
                dtpDate.SelectedDate = servicesContainer.GetInstance<ExpirationsRemindersService>().getNextReminder(transaction.id) ?? transaction.date;
                cbPeriodTransaction.SelectedValue = transaction.periodsRemindersid;
                cbAccount.SelectedValue = transaction.accountid;
                cbPerson.SelectedValue = transaction.personid;
                txtMemo.Text = transaction.memo;
                cbCategory.SelectedValue = transaction.categoryid;
                txtAmount.Value = transaction.amount;
                cbTag.SelectedValue = transaction.tagid;
                cbTransactionStatus.SelectedValue = transaction.transactionStatusid;
            }
            else
            {

                dtpDate.SelectedDate = DateTime.Now;

                if (accountidDefault != null)
                {
                    cbAccount.SelectedValue = accountidDefault;
                }
                else
                {
                    cbAccount.SelectedValue = null;
                }

                cbPerson.SelectedValue = null;
                cbCategory.SelectedValue = null;
                txtMemo.Text = null;
                txtAmount.Value = null;
                cbTag.SelectedValue = null;
                cbTransactionStatus.SelectedValue = (int)TransactionsStatusService.eTransactionsTypes.Pending;

                dtpDate.Focus();
            }
        }

        private void updateTransaction()
        {
            if (transaction == null)
            {
                transaction = new TransactionsReminders();
            }

            transaction.date = dtpDate.SelectedDate;

            transaction.periodsRemindersid = (int)cbPeriodTransaction.SelectedValue;
            transaction.periodsReminders = servicesContainer.GetInstance<PeriodsRemindersService>().getByID(transaction.periodsRemindersid);

            transaction.accountid = (int)cbAccount.SelectedValue;
            transaction.account = servicesContainer.GetInstance<AccountsService>().getByID(transaction.accountid);

            if (cbPerson.SelectedValue != null)
            {
                transaction.personid = (int)cbPerson.SelectedValue;
                transaction.person = servicesContainer.GetInstance<PersonsService>().getByID(transaction.personid);
            }

            transaction.memo = txtMemo.Text;

            transaction.categoryid = (int)cbCategory.SelectedValue;
            transaction.category = servicesContainer.GetInstance<CategoriesService>().getByID(transaction.categoryid);

            if (txtAmount.Value > 0)
            {
                transaction.amountIn = txtAmount.Value;
                transaction.amountOut = 0;
            }
            else
            {
                transaction.amountOut = -txtAmount.Value;
                transaction.amountIn = 0;
            }

            if (cbTag.SelectedValue != null)
            {
                transaction.tagid = (int)cbTag.SelectedValue;
                transaction.tag = servicesContainer.GetInstance<TagsService>().getByID(transaction.tagid);
            }

            transaction.transactionStatusid = (int)cbTransactionStatus.SelectedValue;
            transaction.transactionStatus = servicesContainer.GetInstance<TransactionsStatusService>().getByID(transaction.transactionStatusid);
        }

        private void loadComboBox()
        {
            cbAccount.ItemsSource = servicesContainer.GetInstance<AccountsService>().getAll();
            cbPerson.ItemsSource = servicesContainer.GetInstance<PersonsService>().getAll();
            cbCategory.ItemsSource = servicesContainer.GetInstance<CategoriesService>().getAll();
            cbTag.ItemsSource = servicesContainer.GetInstance<TagsService>().getAll();
            cbTransactionStatus.ItemsSource = servicesContainer.GetInstance<TransactionsStatusService>().getAll();
            cbPeriodTransaction.ItemsSource = servicesContainer.GetInstance<PeriodsRemindersService>().getAll();
        }

        private bool saveTransaction()
        {
            if (isTransactionValid())
            {
                if (MessageBox.Show("Se va a proceder a guardar el movimiento", "inserción movimiento", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    updateTransaction();
                    if (transaction != null)
                    {
                        servicesContainer.GetInstance<TransactionsRemindersService>().saveChanges(transaction);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool isTransactionValid()
        {
            String errorMessage = "";
            bool valid = true;

            if (dtpDate.SelectedDate == null)
            {
                errorMessage += "- Fecha\n";
                valid = false;
            }

            if (cbPeriodTransaction.SelectedValue == null)
            {
                errorMessage += "- Periodificación\n";
                valid = false;
            }

            if (cbAccount.SelectedValue == null)
            {
                errorMessage += "- Cuenta\n";
                valid = false;
            }

            if (cbCategory.SelectedValue == null)
            {
                errorMessage += "- Categoría\n";
                valid = false;
            }

            if (txtAmount.Value == null)
            {
                errorMessage += "- Cantidad\n";
                valid = false;
            }

            if (cbTransactionStatus.SelectedValue == null)
            {
                errorMessage += "- Estado\n";
                valid = false;
            }

            if (errorMessage != "")
            {
                errorMessage = "Tiene que rellenar los siguiente campos para continuar:\n" + errorMessage.TrimEnd('\n');
                MessageBox.Show(errorMessage, "Error de validación", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return valid;
        }

        #endregion

    }
}
