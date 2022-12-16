using BBDDLib.Models;
using GastosRYC.BBDDLib.Services;
using Syncfusion.Windows.Controls.RichTextBoxAdv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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
    public partial class frmTransaction : Window
    {

        private Transactions? transaction;
        private readonly int? accountidDefault;

        private readonly AccountsService accountsService = new AccountsService();
        private readonly CategoriesService categoriesService = new CategoriesService();
        private readonly PersonsService personService = new PersonsService();
        private readonly TagsService tagsService = new TagsService();
        private readonly TransactionsService transactionsService = new TransactionsService();
        private readonly SplitsService splitsService = new SplitsService();
        private readonly TransactionsStatusService transactionsStatusService = new TransactionsStatusService();

        public frmTransaction(Transactions transaction, int accountidDefault)
        {
            InitializeComponent();
            this.transaction = transaction;
            this.accountidDefault = accountidDefault;
        }

        public frmTransaction(Transactions transaction)
        {
            InitializeComponent();
            this.transaction = transaction;
        }

        public frmTransaction(int accountidDefault)
        {
            InitializeComponent();
            this.accountidDefault = accountidDefault;
        }

        public frmTransaction()
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

                cbTransactionStatus.SelectedValue = (int)TransactionsStatusService.eTransactionsTypes.Pending;
            }
        }

        private void updateTransaction()
        {
            if (transaction == null)
            {
                transaction = new Transactions();
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
            transaction.transactionStatus = transactionsStatusService.getByID(transaction.transactionStatusid);
        }

        private void loadComboBox()
        {
            cbAccount.ItemsSource = accountsService.getAll();
            cbPerson.ItemsSource = personService.getAll();
            cbCategory.ItemsSource = categoriesService.getAll();
            cbTag.ItemsSource = tagsService.getAll();
            cbTransactionStatus.ItemsSource = transactionsStatusService.getAll();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (isTransactionValid())
            {
                updateTransaction();
                if (transaction != null)
                {
                    transactionsService.saveChanges(transaction);
                }

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
            if (transaction == null)
            {
                if (MessageBox.Show("Se va a proceder a guardar el movimiento", "inserción movimiento", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    updateTransaction();
                    if (transaction != null)
                    {
                        transactionsService.saveChanges(transaction);
                    }
                }
                else
                {
                    MessageBox.Show("Sin guardar no se puede realizar un split", "inserción movimiento");
                    return;
                }
            }

            frmSplits frm = new frmSplits(transaction);
            frm.ShowDialog();
            transactionsService.updateSplits(transaction);
            loadTransaction();
        }

    }
}
