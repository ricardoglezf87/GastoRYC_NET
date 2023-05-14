using BOLib.Services;
using BOLib.Models;
using System;
using System.Windows;
using System.Windows.Input;
using static BOLib.Extensions.WindowsExtension;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para FrmTransaction.xaml
    /// </summary>
    public partial class FrmTransactionReminders : Window
    {
        #region Variables

        private TransactionsReminders? transaction;
        private readonly int? accountidDefault;
        private readonly TagsService tagsService;
        private readonly PersonsService personsService;
        private readonly CategoriesService categoriesService;
        private readonly AccountsService accountsService;
        private readonly TransactionsRemindersService transactionsRemindersService;
        private readonly ExpirationsRemindersService expirationsRemindersService;
        private readonly InvestmentProductsService investmentProductsService;
        private readonly PeriodsRemindersService periodsRemindersService;
        private readonly TransactionsStatusService transactionsStatusService;

        public eWindowsResult windowsResult { set; get; }

        #endregion

        #region Constructors

        public FrmTransactionReminders()
        {
            InitializeComponent();
            personsService = InstanceBase<PersonsService>.Instance;
            tagsService = InstanceBase<TagsService>.Instance;
            transactionsRemindersService = InstanceBase<TransactionsRemindersService>.Instance;
            categoriesService = InstanceBase<CategoriesService>.Instance;
            accountsService = InstanceBase<AccountsService>.Instance;
            expirationsRemindersService = InstanceBase<ExpirationsRemindersService>.Instance;
            investmentProductsService = InstanceBase<InvestmentProductsService>.Instance;
            periodsRemindersService = InstanceBase<PeriodsRemindersService>.Instance;
            transactionsStatusService = InstanceBase<TransactionsStatusService>.Instance;
        }

        public FrmTransactionReminders(TransactionsReminders transaction, int accountidDefault) :
            this()
        {
            this.transaction = transaction;
            this.accountidDefault = accountidDefault;
        }

        public FrmTransactionReminders(TransactionsReminders? transaction) :
            this()
        {
            this.transaction = transaction;
        }

        public FrmTransactionReminders(int accountidDefault) :
            this()
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
                windowsResult = eWindowsResult.Sucess;
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

            FrmSplitsRemindersList frm = new FrmSplitsRemindersList(transaction);
            frm.ShowDialog();
            transactionsRemindersService.updateSplitsReminders(transaction);
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
                dtpDate.SelectedDate = expirationsRemindersService.getNextReminder(transaction.id) ?? transaction.date;
                cbPeriodTransaction.SelectedValue = transaction.periodsRemindersid;
                cbAccount.SelectedValue = transaction.accountid;
                cbPerson.SelectedValue = transaction.personid;
                txtMemo.Text = transaction.memo;
                cbCategory.SelectedValue = transaction.categoryid;
                txtAmount.Value = transaction.amount;
                cbTag.SelectedValue = transaction.tagid;
                cbTransactionStatus.SelectedValue = transaction.transactionStatusid;
                chkAutoregister.IsChecked = transaction.autoRegister ?? false;
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
            transaction.periodsReminders = periodsRemindersService.getByID(transaction.periodsRemindersid);

            transaction.accountid = (int)cbAccount.SelectedValue;
            transaction.account = accountsService.getByID(transaction.accountid);

            if (cbPerson.SelectedValue != null)
            {
                transaction.personid = (int)cbPerson.SelectedValue;
                transaction.person = personsService.getByID(transaction.personid);
            }

            transaction.memo = txtMemo.Text;

            transaction.categoryid = (int)cbCategory.SelectedValue;
            transaction.category = categoriesService.getByID(transaction.categoryid);

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
                transaction.tag = tagsService.getByID(transaction.tagid);
            }

            transaction.transactionStatusid = (int)cbTransactionStatus.SelectedValue;

            transaction.autoRegister = chkAutoregister.IsChecked ?? false;

            transaction.transactionStatus = transactionsStatusService.getByID(transaction.transactionStatusid);
        }

        private void loadComboBox()
        {
            cbAccount.ItemsSource = accountsService.getAll();
            cbPerson.ItemsSource = personsService.getAll();
            cbCategory.ItemsSource = categoriesService.getAll();
            cbTag.ItemsSource = tagsService.getAll();
            cbTransactionStatus.ItemsSource = transactionsStatusService.getAll();
            cbPeriodTransaction.ItemsSource = periodsRemindersService.getAll();
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
                        transactionsRemindersService.saveChanges(transaction);
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
