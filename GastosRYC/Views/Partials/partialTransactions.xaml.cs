using BOLib.Extensions;
using BOLib.Models;
using BOLib.ModelsView;
using BOLib.Services;
using Syncfusion.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static BOLib.Extensions.WindowsExtension;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para partialTransactions.xaml
    /// </summary>
    public partial class PartialTransactions : Page
    {

        #region Variables

        private AccountsView? accountSelected;
        private readonly MainWindow parentForm;
        private readonly SplitsService splitsService;
        private readonly TransactionsService transactionsService;

        #endregion

        #region Constructor

        public PartialTransactions(MainWindow _parentForm)
        {
            InitializeComponent();
            parentForm = _parentForm;
            splitsService = InstanceBase<SplitsService>.Instance;
            transactionsService = InstanceBase<TransactionsService>.Instance;

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
            transactionsService.updateTransactionAfterSplits(transactions);
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
                foreach (Transactions transactions in gvTransactions.SelectedItems)
                {
                    TransactionsReminders transactionsReminders = new();
                    transactionsReminders.date = transactions.date;
                    transactionsReminders.accountid = transactions.accountid;
                    transactionsReminders.personid = transactions.personid;
                    transactionsReminders.categoryid = transactions.categoryid;
                    transactionsReminders.memo = transactions.memo;
                    transactionsReminders.amountIn = transactions.amountIn;
                    transactionsReminders.amountOut = transactions.amountOut;
                    transactionsReminders.tagid = transactions.tagid;
                    transactionsReminders.transactionStatusid = (int)TransactionsStatusService.eTransactionsTypes.Pending;

                    FrmTransactionReminders frm = new(transactionsReminders);
                    frm.ShowDialog();
                    if (frm.windowsResult == eWindowsResult.Sucess)
                        MessageBox.Show("Recordatorio creado.", "Crear Recordatorio");
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
                    transactionsService.update(transactions);
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
                    transactionsService.update(transactions);
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
                    transactionsService.update(transactions);
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

        public void refreshBalanceTransactions()
        {
            Decimal? balanceTotal = 0;

            if (gvTransactions.View != null)
            {

                Syncfusion.UI.Xaml.Grid.
                    GridQueryableCollectionViewWrapper col = (Syncfusion.UI.Xaml.Grid.
                        GridQueryableCollectionViewWrapper)gvTransactions.View;

                balanceTotal = accountSelected != null
                    ? (decimal?)col.ViewSource.Where("accountid", accountSelected.id, Syncfusion.Data.FilterType.Equals, false).Sum("amount")
                    : (decimal?)col.ViewSource.Sum("amount");

                foreach (Transactions t in col.ViewSource)
                {
                    if (t.amount != null)
                    {
                        if (accountSelected != null && accountSelected.id == t.account?.id)
                        {
                            t.balance = balanceTotal;
                            balanceTotal -= t.amount;
                        }
                        else if (accountSelected == null)
                        {
                            t.balance = balanceTotal;
                            balanceTotal -= t.amount;
                        }

                    }
                }
            }
        }

        public void loadTransactions()
        {
            gvTransactions.ItemsSource = transactionsService.getAll();
            ApplyFilters(accountSelected);
        }

        public bool accountFilter(object? o)
        {
            Transactions? p = (o as Transactions);
            return p == null ? false : p.account?.id == accountSelected?.id;
        }

        public void ApplyFilters(AccountsView? _accountSelected = null)
        {
            accountSelected = _accountSelected;
            if (gvTransactions.View != null)
            {
                if (_accountSelected != null)
                {
                    gvTransactions.View.Filter = accountFilter;
                    gvTransactions.Columns["account.description"].IsHidden = true;

                    if (accountSelected.accountsTypesid == (int)AccountsTypesService.eAccountsTypes.Invests)
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
                    gvTransactions.View.Filter = null;
                    gvTransactions.Columns["account.description"].IsHidden = false;
                }

                gvTransactions.View.RefreshFilter();
                refreshBalanceTransactions();
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
                    List<Splits> lSplits = transactions.splits;
                    for (int i = 0; i < lSplits.Count; i++)
                    {
                        Splits splits = lSplits[i];
                        if (splits.tranferid != null)
                        {
                            transactionsService.delete(transactionsService.getByID(splits.tranferid));
                        }

                        splitsService.delete(splits);
                    }
                }

                if (transactions.tranferid != null)
                {
                    transactionsService.delete(transactionsService.getByID(transactions.tranferid));
                }

                transactionsService.delete(transactions);
            }
        }


        #endregion       
    }
}
