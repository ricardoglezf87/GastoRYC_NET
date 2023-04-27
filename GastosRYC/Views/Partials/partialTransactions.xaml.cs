using BBDDLib.Models;
using GastosRYC.BBDDLib.Services;
using GastosRYC.Extensions;
using Syncfusion.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para partialTransactions.xaml
    /// </summary>
    public partial class PartialTransactions : Page
    {

        #region Variables

        private readonly SimpleInjector.Container servicesContainer;
        private Accounts? accountSelected;
        private readonly MainWindow parentForm;

        #endregion

        #region Constructor

        public PartialTransactions(SimpleInjector.Container _servicesContainer, MainWindow _parentForm)
        {
            InitializeComponent();
            servicesContainer = _servicesContainer;
            parentForm = _parentForm;
        }

        #endregion

        #region Events

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadTransactions();
            parentForm.refreshBalance();
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
            FrmSplitsList frm = new FrmSplitsList(transactions, servicesContainer);
            frm.ShowDialog();
            servicesContainer.GetInstance<TransactionsService>().updateTransactionAfterSplits(transactions);
            loadTransactions();
            parentForm.refreshBalance();
        }

        private void gvTransactions_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Transactions transactions in e.Items)
            {
                removeTransaction(transactions);
            }

            parentForm.loadAccounts();
            loadTransactions();
            parentForm.refreshBalance();
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
                    TransactionsReminders transactionsReminders = new TransactionsReminders();
                    transactionsReminders.date = transactions.date;
                    transactionsReminders.accountid = transactions.accountid;
                    transactionsReminders.personid = transactions.personid;
                    transactionsReminders.categoryid = transactions.categoryid;
                    transactionsReminders.memo = transactions.memo;
                    transactionsReminders.amountIn = transactions.amountIn;
                    transactionsReminders.amountOut = transactions.amountOut;
                    transactionsReminders.tagid = transactions.tagid;
                    transactionsReminders.transactionStatusid = (int)TransactionsStatusService.eTransactionsTypes.Pending;

                    FrmTransactionReminders frm = new FrmTransactionReminders(transactionsReminders, servicesContainer);
                    frm.ShowDialog();
                }

                MessageBox.Show("Recordatorio creado.", "Crear Recordatorio");

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
                FrmTransaction frm = new FrmTransaction((Transactions)gvTransactions.CurrentItem, servicesContainer);
                frm.ShowDialog();
                parentForm.loadAccounts();
                loadTransactions();
                parentForm.refreshBalance();
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
                parentForm.refreshBalance();
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
                    servicesContainer.GetInstance<TransactionsService>().update(transactions);
                }
                loadTransactions();
                parentForm.refreshBalance();
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
                    servicesContainer.GetInstance<TransactionsService>().update(transactions);
                }
                loadTransactions();
                parentForm.refreshBalance();
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
                    servicesContainer.GetInstance<TransactionsService>().update(transactions);
                }
                loadTransactions();
                parentForm.refreshBalance();
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

                if (accountSelected != null)
                    balanceTotal = (decimal?)col.ViewSource.Where("accountid", accountSelected.id, Syncfusion.Data.FilterType.Equals, false).Sum("amount");
                else
                    balanceTotal = (decimal?)col.ViewSource.Sum("amount");

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
            gvTransactions.ItemsSource = servicesContainer.GetInstance<TransactionsService>().getAll();
            ApplyFilters(accountSelected);
        }

        public bool accountFilter(object? o)
        {
            Transactions? p = (o as Transactions);
            if (p == null)
                return false;
            else
                if (p.account?.id == accountSelected?.id)
                return true;
            else
                return false;
        }

        public void ApplyFilters(Accounts? _accountSelected = null)
        {
            accountSelected = _accountSelected;
            if (gvTransactions.View != null)
            {
                if (_accountSelected != null)
                {
                    gvTransactions.View.Filter = accountFilter;
                    gvTransactions.Columns["account.description"].IsHidden = true;
                }
                else
                {
                    gvTransactions.View.Filter = null;
                    gvTransactions.Columns["account.description"].IsHidden = false;
                }

                gvTransactions.View.RefreshFilter();
                parentForm.refreshBalance();
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
                            servicesContainer.GetInstance<TransactionsService>().delete(servicesContainer.GetInstance<TransactionsService>().getByID(splits.tranferid));
                        }

                        servicesContainer.GetInstance<SplitsService>().delete(splits);
                    }
                }

                if (transactions.tranferid != null)
                {
                    servicesContainer.GetInstance<TransactionsService>().delete(servicesContainer.GetInstance<TransactionsService>().getByID(transactions.tranferid));
                }

                servicesContainer.GetInstance<TransactionsService>().delete(transactions);
            }
        }


        #endregion       
    }
}
