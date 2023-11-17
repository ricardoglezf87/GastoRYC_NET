using GARCA.Data.Services;
using GARCA.Models;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static GARCA.Data.IOC.DependencyConfig;
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
            accountidDefault = null;
        }

        public FrmTransactionReminders(TransactionsReminders? transaction) :
            this()
        {
            this.transaction = transaction;
        }

        #endregion

        #region Events

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadComboBox();
            await LoadTransaction();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (await SaveTransaction())
            {
                WindowsResult = EWindowsResult.Sucess;
                Close();
            }
        }

        private async void btnSplit_Click(object sender, RoutedEventArgs e)
        {
            if (transaction == null && !await SaveTransaction())
            {
                MessageBox.Show("Sin guardar no se puede realizar un split", "inserción movimiento");
                return;
            }

            FrmSplitsRemindersList frm = new(transaction);
            frm.ShowDialog();
            await iTransactionsRemindersService.UpdateSplitsReminders(transaction);
            await LoadTransaction();
        }

        private async void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    if (await SaveTransaction())
                    {
                        transaction = null;
                        await LoadTransaction();
                    }
                    break;
                case Key.F2:
                    await SaveTransaction();
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

        private async Task LoadTransaction()
        {
            if (transaction != null)
            {
                dtpDate.SelectedDate = (await iExpirationsRemindersService.GetNextReminder(transaction.Id)) ?? transaction.Date;
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

                cbAccount.SelectedValue = accountidDefault != null ? accountidDefault : (object?)null;

                cbPerson.SelectedIndex = -1;
                cbCategory.SelectedIndex = -1;
                txtMemo.Text = null;
                txtAmount.Value = null;
                cbTag.SelectedIndex = -1;
                cbTransactionStatus.SelectedValue = (int)TransactionsStatusService.ETransactionsTypes.Pending;

                dtpDate.Focus();
            }
        }

        private async Task UpdateTransaction()
        {
            transaction ??= new TransactionsReminders();

            transaction.Date = dtpDate.SelectedDate;

            transaction.PeriodsRemindersid = (int)cbPeriodTransaction.SelectedValue;
            transaction.PeriodsReminders = await iPeriodsReminderService.GetById(transaction.PeriodsRemindersid ?? -99);

            transaction.Accountid = (int)cbAccount.SelectedValue;
            transaction.Account = await iAccountsService.GetById(transaction.Accountid ?? -99);

            if (cbPerson.SelectedValue != null)
            {
                transaction.Personid = (int)cbPerson.SelectedValue;
                transaction.Person = await iPersonsService.GetById(transaction.Personid ?? -99);
            }

            transaction.Memo = txtMemo.Text;

            transaction.Categoryid = (int)cbCategory.SelectedValue;
            transaction.Category = await iCategoriesService.GetById(transaction.Categoryid ?? -99);

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
                transaction.Tag = await iTagsService.GetById(transaction.Tagid ?? -99);
            }

            transaction.TransactionStatusid = (int)cbTransactionStatus.SelectedValue;

            transaction.AutoRegister = chkAutoregister.IsChecked ?? false;

            transaction.TransactionStatus = await iTransactionsStatusService.GetById(transaction.TransactionStatusid ?? -99);
        }

        private async Task LoadComboBox()
        {
            cbAccount.ItemsSource = await iAccountsService.GetAll();
            cbPerson.ItemsSource = await iPersonsService.GetAll();
            cbCategory.ItemsSource = await iCategoriesService.GetAll();
            cbTag.ItemsSource = await iTagsService.GetAll();
            cbTransactionStatus.ItemsSource = await iTransactionsStatusService.GetAll();
            cbPeriodTransaction.ItemsSource = await iPeriodsReminderService.GetAll();
        }

        private async Task<bool> SaveTransaction()
        {
            if (IsTransactionValid())
            {
                if (MessageBox.Show("Se va a proceder a guardar el movimiento", "inserción movimiento", MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await UpdateTransaction();
                    if (transaction != null)
                    {
                        await iTransactionsRemindersService.SaveChanges(transaction);
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
