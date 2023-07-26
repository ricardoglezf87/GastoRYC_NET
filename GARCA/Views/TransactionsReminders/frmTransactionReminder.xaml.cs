using GARCA.BO.Models;
using GARCA.BO.Services;
using System;
using System.Windows;
using System.Windows.Input;
using static GARCA.BO.Extensions.WindowsExtension;

namespace GARCA.Views
{
    /// <summary>
    /// Lógica de interacción para FrmTransaction.xaml
    /// </summary>
    public partial class FrmTransactionReminders : Window
    {
        #region Variables

        private TransactionsReminders? transaction;
        private readonly int? accountidDefault;

        public eWindowsResult windowsResult { set; get; }

        #endregion

        #region Constructors

        public FrmTransactionReminders()
        {
            InitializeComponent();
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

            FrmSplitsRemindersList frm = new(transaction);
            frm.ShowDialog();
            TransactionsRemindersService.Instance.updateSplitsReminders(transaction);
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

        private void dtpDate_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Tab or Key.Enter)
            {
                if (dtpDate.Text.Length == 4)
                {
                    dtpDate.Text = dtpDate.Text.Substring(0, 2) + "/" + dtpDate.Text.Substring(2, 2);
                }
                else if (dtpDate.Text.Length == 6)
                {
                    dtpDate.Text = dtpDate.Text.Substring(0, 2) + "/" + dtpDate.Text.Substring(2, 2) + "/" + dtpDate.Text.Substring(4, 2);
                }

                if (e.Key == Key.Enter)
                {
                    cbAccount.Focus();
                }
            }
        }

        #endregion

        #region Functions

        private void loadTransaction()
        {
            if (transaction != null)
            {
                dtpDate.SelectedDate = ExpirationsRemindersService.Instance.getNextReminder(transaction.id) ?? transaction.date;
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

                cbAccount.SelectedValue = accountidDefault != null ? accountidDefault : (object?)null;

                cbPerson.SelectedIndex = -1;
                cbCategory.SelectedIndex = -1;
                txtMemo.Text = null;
                txtAmount.Value = null;
                cbTag.SelectedIndex = -1;
                cbTransactionStatus.SelectedValue = (int)TransactionsStatusService.eTransactionsTypes.Pending;

                dtpDate.Focus();
            }
        }

        private void updateTransaction()
        {
            transaction ??= new TransactionsReminders();

            transaction.date = dtpDate.SelectedDate;

            transaction.periodsRemindersid = (int)cbPeriodTransaction.SelectedValue;
            transaction.periodsReminders = PeriodsRemindersService.Instance.getByID(transaction.periodsRemindersid);

            transaction.accountid = (int)cbAccount.SelectedValue;
            transaction.account = AccountsService.Instance.getByID(transaction.accountid);

            if (cbPerson.SelectedValue != null)
            {
                transaction.personid = (int)cbPerson.SelectedValue;
                transaction.person = PersonsService.Instance.getByID(transaction.personid);
            }

            transaction.memo = txtMemo.Text;

            transaction.categoryid = (int)cbCategory.SelectedValue;
            transaction.category = CategoriesService.Instance.getByID(transaction.categoryid);

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
                transaction.tag = TagsService.Instance.getByID(transaction.tagid);
            }

            transaction.transactionStatusid = (int)cbTransactionStatus.SelectedValue;

            transaction.autoRegister = chkAutoregister.IsChecked ?? false;

            transaction.transactionStatus = TransactionsStatusService.Instance.getByID(transaction.transactionStatusid);
        }

        private void loadComboBox()
        {
            cbAccount.ItemsSource = AccountsService.Instance.getAll();
            cbPerson.ItemsSource = PersonsService.Instance.getAll();
            cbCategory.ItemsSource = CategoriesService.Instance.getAll();
            cbTag.ItemsSource = TagsService.Instance.getAll();
            cbTransactionStatus.ItemsSource = TransactionsStatusService.Instance.getAll();
            cbPeriodTransaction.ItemsSource = PeriodsRemindersService.Instance.getAll();
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
                        TransactionsRemindersService.Instance.saveChanges(transaction);
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
