using GARCA.Utlis.Extensions;
using GARCA.BO.Models;
using GARCA.View.ViewModels;
using GARCA.BO.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static GARCA.Utlis.Extensions.WindowsExtension;
using GARCA.View.ViewModels;
using GARCA.Utils.IOC;

namespace GARCA.View.Views
{
    /// <summary>
    /// Lógica de interacción para partialTransactions.xaml
    /// </summary>
    public partial class PartialTransactions : Page
    {

        #region Variables

        private readonly MainWindow parentForm;

        #endregion

        #region Constructor

        public PartialTransactions(MainWindow _parentForm)
        {
            InitializeComponent();
            parentForm = _parentForm;
        }

        #endregion

        #region Events

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadTransactions();
        }

        private void gvTransactions_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer eliminar este movimiento?", "Eliminación movimiento", MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void ButtonSplit_Click(object sender, RoutedEventArgs e)
        {
            Transactions transactions = (Transactions)gvTransactions.SelectedItem;
            FrmSplitsList frm = new(transactions);
            frm.ShowDialog();
            DependencyConfig.iTransactionsService.updateTransactionAfterSplits(transactions);
            loadTransactions();
            parentForm.loadAccounts();
        }

        private void gvTransactions_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Transactions transactions in e.Items)
            {
                removeTransaction(transactions);
            }

            loadTransactions();
            parentForm.loadAccounts();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Implementar funcionalidad
            MessageBox.Show("Funcionalidad no implementada");
        }

        private void btnAddReminder_Click(object sender, RoutedEventArgs e)
        {
            if (gvTransactions.SelectedItems != null && gvTransactions.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Se va a proceder a crear los recordatorios", "Crear Recordatorio", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (Transactions transactions in gvTransactions.SelectedItems)
                    {
                        TransactionsReminders? transactionsReminders = new();
                        transactionsReminders.date = transactions.date;
                        transactionsReminders.periodsRemindersid = (int)PeriodsRemindersService.ePeriodsReminders.Monthly;
                        transactionsReminders.accountid = transactions.accountid;
                        transactionsReminders.personid = transactions.personid;
                        transactionsReminders.categoryid = transactions.categoryid;
                        transactionsReminders.memo = transactions.memo;
                        transactionsReminders.amountIn = transactions.amountIn;
                        transactionsReminders.amountOut = transactions.amountOut;
                        transactionsReminders.tagid = transactions.tagid;
                        transactionsReminders.transactionStatusid = (int)TransactionsStatusService.eTransactionsTypes.Pending;

                        transactionsReminders = DependencyConfig.iTransactionsRemindersService.update(transactionsReminders);

                        foreach (Splits? splits in DependencyConfig.iSplitsService.getbyTransactionid(transactions.id))
                        {
                            SplitsReminders splitsReminders = new();
                            splitsReminders.transactionid = transactionsReminders.id;
                            splitsReminders.categoryid = splits.categoryid;
                            splitsReminders.memo = splits.memo;
                            splitsReminders.amountIn = splits.amountIn;
                            splitsReminders.amountOut = splits.amountOut;
                            splitsReminders.tagid = splits.tagid;
                            DependencyConfig.iSplitsRemindersService.update(splitsReminders);
                        }

                        FrmTransactionReminders frm = new(transactionsReminders);
                        frm.ShowDialog();
                        if (frm.windowsResult == eWindowsResult.Sucess)
                        {
                            MessageBox.Show("Recordatorio creado.", "Crear Recordatorio");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No se ha creado el recordatorios.", "Crear Recordatorio");
                }
            }
            else
            {
                MessageBox.Show("Tiene que seleccionar alguna línea.", "Crear Recordatorio");
            }
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Implementar funcionalidad
            MessageBox.Show("Funcionalidad no implementada");
        }

        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Implementar funcionalidad
            MessageBox.Show("Funcionalidad no implementada");
        }

        private void gvTransactions_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (gvTransactions.CurrentItem != null)
            {
                FrmTransaction frm = new((Transactions)gvTransactions.CurrentItem);
                frm.ShowDialog();
                loadTransactions();
                parentForm.loadAccounts();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (gvTransactions.SelectedItems != null && gvTransactions.SelectedItems.Count > 0)
            {
                foreach (Transactions transactions in gvTransactions.SelectedItems)
                {
                    removeTransaction(transactions);
                }
                loadTransactions();
                parentForm.loadAccounts();
            }
            else
            {
                MessageBox.Show("Tiene que seleccionar alguna línea.", "Cambio estado movimieno");
            }
        }

        private void btnPending_Click(object sender, RoutedEventArgs e)
        {
            if (gvTransactions.SelectedItems != null && gvTransactions.SelectedItems.Count > 0)
            {
                foreach (Transactions transactions in gvTransactions.SelectedItems)
                {
                    transactions.transactionStatusid = (int)TransactionsStatusService.eTransactionsTypes.Pending;
                    transactions.transactionStatus = DependencyConfig.iTransactionsStatusService.getByID(transactions.transactionStatusid);
                    DependencyConfig.iTransactionsService.update(transactions);
                }
                loadTransactions();
            }
            else
            {
                MessageBox.Show("Tiene que seleccionar alguna línea.", "Cambio estado movimieno");
            }
        }

        private void btnProvisional_Click(object sender, RoutedEventArgs e)
        {
            if (gvTransactions.SelectedItems != null && gvTransactions.SelectedItems.Count > 0)
            {
                foreach (Transactions transactions in gvTransactions.SelectedItems)
                {
                    transactions.transactionStatusid = (int)TransactionsStatusService.eTransactionsTypes.Provisional;
                    transactions.transactionStatus = DependencyConfig.iTransactionsStatusService.getByID(transactions.transactionStatusid);
                    DependencyConfig.iTransactionsService.update(transactions);
                }
                loadTransactions();
            }
            else
            {
                MessageBox.Show("Tiene que seleccionar alguna línea.", "Cambio estado movimieno");
            }
        }

        private void btnReconciled_Click(object sender, RoutedEventArgs e)
        {
            if (gvTransactions.SelectedItems != null && gvTransactions.SelectedItems.Count > 0)
            {
                foreach (Transactions transactions in gvTransactions.SelectedItems)
                {
                    transactions.transactionStatusid = (int)TransactionsStatusService.eTransactionsTypes.Reconciled;
                    transactions.transactionStatus = DependencyConfig.iTransactionsStatusService.getByID(transactions.transactionStatusid);
                    DependencyConfig.iTransactionsService.update(transactions);
                }
                loadTransactions();
            }
            else
            {
                MessageBox.Show("Tiene que seleccionar alguna línea.", "Cambio estado movimieno");
            }
        }

        #endregion

        #region Functions        

        public void loadTransactions()
        {
            setColumnVisibility(TransactionViewModel.accountsSelected);
        }

        public void setColumnVisibility(AccountsView? _accountSelected = null)
        {
            TransactionViewModel.accountsSelected = _accountSelected;

            if (gvTransactions.View != null)
            {
                gvTransactions.View.Refresh();

                if (_accountSelected != null)
                {
                    gvTransactions.Columns["account.description"].IsHidden = true;

                    if (TransactionViewModel.accountsSelected.accountsTypesid == (int)AccountsTypesService.eAccountsTypes.Invests)
                    {
                        gvTransactions.Columns["numShares"].IsHidden = false;
                        gvTransactions.Columns["pricesShares"].IsHidden = false;
                        gvTransactions.Columns["amountIn"].IsHidden = true;
                        gvTransactions.Columns["amountOut"].IsHidden = true;
                    }
                    else
                    {
                        gvTransactions.Columns["numShares"].IsHidden = true;
                        gvTransactions.Columns["pricesShares"].IsHidden = true;
                        gvTransactions.Columns["amountIn"].IsHidden = false;
                        gvTransactions.Columns["amountOut"].IsHidden = false;
                    }
                }
                else
                {
                    gvTransactions.Columns["account.description"].IsHidden = false;
                }
            }
        }

        private void removeTransaction(Transactions transactions)
        {
            if (transactions.tranferSplitid != null)
            {
                MessageBox.Show("El movimiento Id: " + transactions.id.ToString() +
                    " de fecha: " + transactions.date.toShortDateString() + " viene de una transferencia desde split, para borrar diríjase al split que lo generó.", "Eliminación movimiento");
            }
            else
            {
                if (transactions.splits != null)
                {
                    List<Splits?>? lSplits = transactions.splits;
                    for (int i = 0; i < lSplits.Count; i++)
                    {
                        Splits? splits = lSplits[i];
                        if (splits.tranferid != null)
                        {
                            DependencyConfig.iTransactionsService.delete(DependencyConfig.iTransactionsService.getByID(splits.tranferid));
                        }

                        DependencyConfig.iSplitsService.delete(splits);
                    }
                }

                if (transactions.tranferid != null)
                {
                    DependencyConfig.iTransactionsService.delete(DependencyConfig.iTransactionsService.getByID(transactions.tranferid));
                }

                DependencyConfig.iTransactionsService.delete(transactions);
            }
        }


        #endregion       
    }
}
