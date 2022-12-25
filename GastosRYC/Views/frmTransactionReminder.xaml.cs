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
    /// Lógica de interacción para frmTransaction.xaml
    /// </summary>
    public partial class frmTransactionReminders : Window
    {

        private TransactionsReminders? transaction;
        private readonly int? accountidDefault;

        private readonly AccountsService accountsService = new AccountsService();
        private readonly CategoriesService categoriesService = new CategoriesService();
        private readonly PersonsService personService = new PersonsService();
        private readonly TagsService tagsService = new TagsService();
        private readonly TransactionsRemindersService transactionsRemindersService = new TransactionsRemindersService();
        private readonly SplitsRemindersService splitsRemindersService = new SplitsRemindersService();
        private readonly TransactionsStatusService transactionsRemindersStatusService = new TransactionsStatusService();

        public frmTransactionReminders(TransactionsReminders transaction, int accountidDefault)
        {
            InitializeComponent();
            this.transaction = transaction;
            this.accountidDefault = accountidDefault;
        }

        public frmTransactionReminders(TransactionsReminders transaction)
        {
            InitializeComponent();
            this.transaction = transaction;
        }

        public frmTransactionReminders(int accountidDefault)
        {
            InitializeComponent();
            this.accountidDefault = accountidDefault;
        }

        public frmTransactionReminders()
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
            transaction.accountid = (int)cbAccount.SelectedValue;
            transaction.account = accountsService.getByID(transaction.accountid);

            if (cbPerson.SelectedValue != null)
            {
                transaction.personid = (int)cbPerson.SelectedValue;
                transaction.person = personService.getByID(transaction.personid);
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
            transaction.transactionStatus = transactionsRemindersStatusService.getByID(transaction.transactionStatusid);
        }

        private void loadComboBox()
        {
            cbAccount.ItemsSource = accountsService.getAll();
            cbPerson.ItemsSource = personService.getAll();
            cbCategory.ItemsSource = categoriesService.getAll();
            cbTag.ItemsSource = tagsService.getAll();
            cbTransactionStatus.ItemsSource = transactionsRemindersStatusService.getAll();
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

            frmSplitsReminders frm = new frmSplitsReminders(transaction);
            frm.ShowDialog();
            transactionsRemindersService.updateSplitsReminders(transaction);
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
