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

        private readonly Transactions? transaction;
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

                transactionsService.saveChanges(transaction);

                this.Close();
            }
        }

        private bool isTransactionValid()
        {
            //    Transactions transactions = (Transactions)e.RowData;
            //    if (transactions != null)
            //    {
            //        if (transactions.date == null)
            //        {
            //            e.IsValid = false;
            //            e.ErrorMessages.Add("date", "Tiene que rellenar la fecha");
            //        }

            //        if (transactions.accountid == null)
            //        {
            //            e.IsValid = false;
            //            e.ErrorMessages.Add("accountid", "Tiene que rellenar la cuenta");
            //        }

            //        if (transactions.categoryid == null)
            //        {
            //            e.IsValid = false;
            //            e.ErrorMessages.Add("categoryid", "Tiene que rellenar la categoria");
            //        }
            //        else
            //        {

            //            if (transactions.categoryid == (int)CategoriesService.eSpecialCategories.Split &&
            //                (transactions.splits == null || transactions.splits.Count == 0))
            //            {
            //                e.IsValid = false;
            //                e.ErrorMessages.Add("categoryid", "No se puede utilizar la categoria Split si no se tiene desglose de movimiento");
            //            }
            //            else if (transactions.categoryid != (int)CategoriesService.eSpecialCategories.Split &&
            //                transactions.splits != null && transactions.splits.Count > 0)
            //            {
            //                e.IsValid = false;
            //                e.ErrorMessages.Add("categoryid", "No se puede utilizar la categoria distinta de Split si se tiene desglose de movimiento");
            //            }
            //            else if (transactions.tranferSplitid != null)
            //            {
            //                e.IsValid = false;
            //                e.ErrorMessages.Add("categoryid", "No se puede cambiar el movimiento cuando la transferencia proviene de un split.");
            //                e.ErrorMessages.Add("amountIn", "No se puede cambiar el movimiento cuando la transferencia proviene de un split.");
            //                e.ErrorMessages.Add("amountOut", "No se puede cambiar el movimiento cuando la transferencia proviene de un split.");
            //            }
            //        }

            //        if (transactions.amountIn == null && transactions.amountOut == null)
            //        {
            //            e.IsValid = false;
            //            e.ErrorMessages.Add("amountIn", "Tiene que rellenar la cantidad");
            //            e.ErrorMessages.Add("amountOut", "Tiene que rellenar la cantidad");
            //        }
            //        else if (transactions.splits != null && transactions.splits.Count > 0
            //            && splitsService.getAmountTotal(transactions) != transactions.amount)
            //        {
            //            e.IsValid = false;
            //            e.ErrorMessages.Add("amountIn", "No se puede cambiar las cantidades cuando el movimiento es un split.");
            //            e.ErrorMessages.Add("amountOut", "No se puede cambiar las cantidades cuando el movimiento es un split.");
            //        }

            //        if (transactions.transactionStatusid == null)
            //        {
            //            e.IsValid = false;
            //            e.ErrorMessages.Add("transactionStatusid", "Tiene que rellenar el estado");
            //        }
            //    }
            return true;
        }

        private void btnSplit_Click(object sender, RoutedEventArgs e)
        {
            //Transactions transactions = (Transactions)gvTransactions.SelectedItem;

            //if (gvTransactions.View.IsAddingNew && transactions == null)
            //{
            //    if (MessageBox.Show("Se va a proceder a guardar el movimiento", "inserción movimiento", MessageBoxButton.YesNo,
            //        MessageBoxImage.Question) == MessageBoxResult.Yes)
            //    {
            //        Syncfusion.UI.Xaml.Grid.GridQueryableCollectionViewWrapper col = (Syncfusion.UI.Xaml.Grid.
            //                   GridQueryableCollectionViewWrapper)gvTransactions.View;
            //        transactions = (Transactions)col.CurrentAddItem;

            //        if (transactions != null)
            //        {
            //            if (transactions.date == null || transactions.accountid == null)
            //            {
            //                MessageBox.Show("Debe rellenar la fecha y cuenta para continuar", "Inserción movimiento");
            //                return;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}

            //frmSplits frm = new frmSplits(transactions);
            //frm.ShowDialog();
            //updateSplits(transactions);
            //loadTransactions();
        }

    }
}
