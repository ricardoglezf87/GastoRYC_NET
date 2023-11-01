using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;
using GARCA.Data.Services;
using System;
using System.Windows;
using System.Windows.Input;
using static GARCA.Utils.Extensions.WindowsExtension;

namespace GARCA.View.Views
{
    /// <summary>
    /// Lógica de interacción para FrmTransaction.xaml
    /// </summary>
    public partial class FrmTransactionReminders : Window
    {
        #region Variables

        private TransactionsReminders? transaction;
        private readonly int? accountidDefault;

        public EWindowsResult WindowsResult { private set; get; }

        #endregion

        #region Constructors

        public FrmTransactionReminders()
        {
            InitializeComponent();
        }

        public FrmTransactionReminders(TransactionsReminders? transaction) :
            this()
        {
            this.transaction = transaction;
        }

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComboBox();
            LoadTransaction();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (SaveTransaction())
            {
                WindowsResult = EWindowsResult.Sucess;
                Close();
            }
        }

        private void btnSplit_Click(object sender, RoutedEventArgs e)
        {
            if (transaction == null && !SaveTransaction())
            {
                MessageBox.Show("Sin guardar no se puede realizar un split", "inserción movimiento");
                return;
            }

            FrmSplitsRemindersList frm = new(transaction);
            frm.ShowDialog();
            iTransactionsRemindersService.UpdateSplitsReminders(transaction);
            LoadTransaction();
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    if (SaveTransaction())
                    {
                        transaction = null;
                        LoadTransaction();
                    }
                    break;
                case Key.F2:
                    SaveTransaction();
                    break;
                case Key.Escape:
                    Close();
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

        private void LoadTransaction()
        {
            if (transaction != null)
            {
                dtpDate.SelectedDate = iExpirationsRemindersService.GetNextReminder(transaction.Id) ?? transaction.Date;
                cbPeriodTransaction.SelectedValue = transaction.PeriodsRemindersid;
                cbAccount.SelectedValue = transaction.Accountid;
                cbPerson.SelectedValue = transaction.Personid;
                txtMemo.Text = transaction.Memo;
                cbCategory.SelectedValue = transaction.Categoryid;
                txtAmount.Value = transaction.Amount;
                cbTag.SelectedValue = transaction.Tagid;
                cbTransactionStatus.SelectedValue = transaction.TransactionStatusid;
                chkAutoregister.IsChecked = transaction.AutoRegister ?? false;
            }
            else
            {

                dtpDate.SelectedDate = DateTime.Now;

                cbAccount.SelectedValue = accountidDefault != null ? accountidDefault : (object)null;

                cbPerson.SelectedIndex = -1;
                cbCategory.SelectedIndex = -1;
                txtMemo.Text = null;
                txtAmount.Value = null;
                cbTag.SelectedIndex = -1;
                cbTransactionStatus.SelectedValue = (int)TransactionsStatusService.ETransactionsTypes.Pending;

                dtpDate.Focus();
            }
        }

        private void UpdateTransaction()
        {
            transaction ??= new TransactionsReminders();

            transaction.Date = dtpDate.SelectedDate;

            transaction.PeriodsRemindersid = (int)cbPeriodTransaction.SelectedValue;
            transaction.PeriodsReminders = iPeriodsReminderService.GetById(transaction.PeriodsRemindersid);

            transaction.Accountid = (int)cbAccount.SelectedValue;
            transaction.Account = iAccountsService.GetById(transaction.Accountid);

            if (cbPerson.SelectedValue != null)
            {
                transaction.Personid = (int)cbPerson.SelectedValue;
                transaction.Person = iPersonsService.GetById(transaction.Personid);
            }

            transaction.Memo = txtMemo.Text;

            transaction.Categoryid = (int)cbCategory.SelectedValue;
            transaction.Category = iCategoriesService.GetById(transaction.Categoryid);

            if (txtAmount.Value > 0)
            {
                transaction.AmountIn = txtAmount.Value;
                transaction.AmountOut = 0;
            }
            else
            {
                transaction.AmountOut = -txtAmount.Value;
                transaction.AmountIn = 0;
            }

            if (cbTag.SelectedValue != null)
            {
                transaction.Tagid = (int)cbTag.SelectedValue;
                transaction.Tag = iTagsService.GetById(transaction.Tagid);
            }

            transaction.TransactionStatusid = (int)cbTransactionStatus.SelectedValue;

            transaction.AutoRegister = chkAutoregister.IsChecked ?? false;

            transaction.TransactionStatus = iTransactionsStatusService.GetById(transaction.TransactionStatusid);
        }

        private void LoadComboBox()
        {
            cbAccount.ItemsSource = iAccountsService.GetAll();
            cbPerson.ItemsSource = iPersonsService.GetAll();
            cbCategory.ItemsSource = iCategoriesService.GetAll();
            cbTag.ItemsSource = iTagsService.GetAll();
            cbTransactionStatus.ItemsSource = iTransactionsStatusService.GetAll();
            cbPeriodTransaction.ItemsSource = iPeriodsReminderService.GetAll();
        }

        private bool SaveTransaction()
        {
            if (IsTransactionValid())
            {
                if (MessageBox.Show("Se va a proceder a guardar el movimiento", "inserción movimiento", MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    UpdateTransaction();
                    if (transaction != null)
                    {
                        iTransactionsRemindersService.SaveChanges(transaction);
                    }
                    return true;
                }

                return false;
            }

            return false;
        }

        private bool IsTransactionValid()
        {
            var errorMessage = "";
            var valid = true;

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
