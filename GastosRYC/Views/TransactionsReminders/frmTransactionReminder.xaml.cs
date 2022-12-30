using BBDDLib.Models;
using GastosRYC.BBDDLib.Services;
using Syncfusion.Windows.Controls.RichTextBoxAdv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para FrmTransaction.xaml
    /// </summary>
    public partial class FrmTransactionReminders : Window
    {

        private TransactionsReminders? transaction;
        private readonly int? accountidDefault;

        public FrmTransactionReminders(TransactionsReminders transaction, int accountidDefault)
        {
            InitializeComponent();
            this.transaction = transaction;
            this.accountidDefault = accountidDefault;
        }

        public FrmTransactionReminders(TransactionsReminders transaction)
        {
            InitializeComponent();
            this.transaction = transaction;
        }

        public FrmTransactionReminders(int accountidDefault)
        {
            InitializeComponent();
            this.accountidDefault = accountidDefault;
        }

        public FrmTransactionReminders()
        {
            InitializeComponent();
            this.transaction = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadComboBox();
            loadTransaction();
        }

        private void loadTransaction()
        {
            if (transaction != null)
            {
                dtpDate.SelectedDate = transaction.date;
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
            transaction.periodsReminders = RYCContextService.periodsRemindersService.getByID(transaction.periodsRemindersid);
            
            transaction.accountid = (int)cbAccount.SelectedValue;
            transaction.account = RYCContextService.accountsService.getByID(transaction.accountid);

            if (cbPerson.SelectedValue != null)
            {
                transaction.personid = (int)cbPerson.SelectedValue;
                transaction.person = RYCContextService.personsService.getByID(transaction.personid);
            }

            transaction.memo = txtMemo.Text;

            transaction.categoryid = (int)cbCategory.SelectedValue;
            transaction.category = RYCContextService.categoriesService.getByID(transaction.categoryid);

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
                transaction.tag = RYCContextService.tagsService.getByID(transaction.tagid);
            }

            transaction.transactionStatusid = (int)cbTransactionStatus.SelectedValue;
            transaction.transactionStatus = RYCContextService.transactionsStatusService.getByID(transaction.transactionStatusid);
        }

        private void loadComboBox()
        {
            cbAccount.ItemsSource = RYCContextService.accountsService.getAll();
            cbPerson.ItemsSource = RYCContextService.personsService.getAll();
            cbCategory.ItemsSource = RYCContextService.categoriesService.getAll();
            cbTag.ItemsSource = RYCContextService.tagsService.getAll();
            cbTransactionStatus.ItemsSource = RYCContextService.transactionsStatusService.getAll();
            cbPeriodTransaction.ItemsSource = RYCContextService.periodsRemindersService.getAll();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (saveTransaction())
            {                
                this.Close();
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

        private void btnSplit_Click(object sender, RoutedEventArgs e)
        {
            if (transaction == null && !saveTransaction())
            {                    
                MessageBox.Show("Sin guardar no se puede realizar un split", "inserción movimiento");
                return;                 
            }

            FrmSplitsRemindersList frm = new FrmSplitsRemindersList(transaction);
            frm.ShowDialog();
            RYCContextService.transactionsRemindersService.updateSplitsReminders(transaction);
            loadTransaction();         
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
                        RYCContextService.transactionsRemindersService.saveChanges(transaction);
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

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch(e.Key)
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
    }
}
