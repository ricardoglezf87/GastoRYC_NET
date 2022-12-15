using GastosRYC.BBDDLib.Services;
using BBDDLib.Models;
using Syncfusion.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using GastosRYC.Views;
using GastosRYC.Extensions;
using System.Collections;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.Data;

//TODO: implementar split

namespace GastosRYC
{
    public partial class MainWindow : Window
    {
        private ICollectionView? viewAccounts;

        private readonly AccountsService accountsService = new AccountsService();        
        private readonly TransactionsService transactionsService = new TransactionsService();
        private readonly SplitsService splitsService = new SplitsService();
    
        public MainWindow()
        {
            InitializeComponent();
        }

        private void reiniciarSaldosCuentas()
        {
            if (lvAccounts.ItemsSource != null)
            {
                foreach (Accounts accounts in lvAccounts.ItemsSource)
                {
                    accounts.balance = 0;
                }
            }
        }

        private void addSaldoCuenta(long? id, Decimal balance)
        {
            if (id != null && lvAccounts.ItemsSource != null
                && viewAccounts != null && viewAccounts.SourceCollection != null)
            {
                List<Accounts> lAccounts = (List<Accounts>)viewAccounts.SourceCollection;
                Accounts? accounts = lAccounts.FirstOrDefault(x => x.id == id);

                if (accounts != null)
                {
                    accounts.balance += balance;
                }
            }
        }

        private void refreshBalance()
        {
            Decimal? balanceTotal = 0;

            if (gvTransactions.View != null)
            {
                reiniciarSaldosCuentas();

                Syncfusion.UI.Xaml.Grid.
                    GridQueryableCollectionViewWrapper col = (Syncfusion.UI.Xaml.Grid.
                        GridQueryableCollectionViewWrapper)gvTransactions.View;

                if (lvAccounts.SelectedItem != null)
                    balanceTotal = (decimal?)col.ViewSource.Where("accountid",
                        ((Accounts)lvAccounts.SelectedItem).id, Syncfusion.Data.FilterType.Equals, false).Sum("amount");
                else
                    balanceTotal = (decimal?)col.ViewSource.Sum("amount");

                foreach (Transactions t in col.ViewSource)
                {
                    if (t.amount != null)
                    {
                        if (lvAccounts.SelectedItem != null && ((Accounts)lvAccounts.SelectedItem).id == t.account?.id)
                        {
                            t.balance = balanceTotal;
                            balanceTotal -= t.amount;
                        }
                        else if (lvAccounts.SelectedItem == null)
                        {
                            t.balance = balanceTotal;
                            balanceTotal -= t.amount;
                        }

                        addSaldoCuenta(t.account?.id, t.amount.Value);
                    }
                }

                viewAccounts?.Refresh();
                autoResizeListView();

            }
        }

        private void autoResizeListView()
        {
            double remainingSpace = lvAccounts.ActualWidth * .93;
            GridView? gv = (lvAccounts.View as GridView);

            if (remainingSpace > 0)
            {
                gv.Columns[0].Width = Math.Ceiling(remainingSpace * .6);
                gv.Columns[1].Width = Math.Ceiling(remainingSpace * .4);
            }
        }

        private void loadAccounts()
        {
            viewAccounts = CollectionViewSource.GetDefaultView(accountsService.getAll());
            lvAccounts.ItemsSource = viewAccounts;
            viewAccounts.GroupDescriptions.Add(new PropertyGroupDescription("accountsTypes"));
            viewAccounts.SortDescriptions.Add(new SortDescription("accountsTypes.id", ListSortDirection.Ascending));            
        }

        private void loadTransactions()
        {
            gvTransactions.ItemsSource = transactionsService.getAll();
            ApplyFilters();
        }

        private void frmInicio_Loaded(object sender, RoutedEventArgs e)
        {
            loadAccounts();
            loadTransactions();
            refreshBalance();
        }

        public void ApplyFilters()
        {
            if (lvAccounts.SelectedValue != null)
            {
                gvTransactions.View.Filter = accountFilter;
            }
            else
            {
                gvTransactions.View.Filter = null;
            }

            gvTransactions.View.RefreshFilter();
            refreshBalance();
        }

        public bool accountFilter(object? o)
        {
            Transactions? p = (o as Transactions);
            if (p == null)
                return false;
            else
                if (p.account?.id == ((Accounts)lvAccounts.SelectedValue)?.id)
                return true;
            else
                return false;
        }

        private void lvAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
            gvTransactions.Columns["account.description"].IsHidden = true;
        }


        private void btnAllAccounts_Click(object sender, RoutedEventArgs e)
        {
            lvAccounts.SelectedItem = null;
            gvTransactions.Columns["account.description"].IsHidden = false;
        }

        private void lvAccounts_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            autoResizeListView();
        }

        private void MenuItem_Accounts_Click(object sender, RoutedEventArgs e)
        {
            frmAccounts frm = new frmAccounts();
            frm.ShowDialog();
            loadAccounts();
            loadTransactions();
            refreshBalance();
        }

        private void MenuItem_Persons_Click(object sender, RoutedEventArgs e)
        {
            frmPersons frm = new frmPersons();
            frm.ShowDialog();
            loadTransactions();
            refreshBalance();
        }

        private void MenuItem_Categories_Click(object sender, RoutedEventArgs e)
        {
            frmCategories frm = new frmCategories();
            frm.ShowDialog();
            loadTransactions();
            refreshBalance();
        }

        private void MenuItem_Salir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Tags_Click(object sender, RoutedEventArgs e)
        {
            frmTags frm = new frmTags();
            frm.ShowDialog();
            loadTransactions();
            refreshBalance();
        }
                
        private void ButtonSplit_Click(object sender, RoutedEventArgs e)
        {
            Transactions transactions = (Transactions)gvTransactions.SelectedItem;

            if(gvTransactions.View.IsAddingNew && transactions == null)
            {
                if (MessageBox.Show("Se va a proceder a guardar el movimiento", "inserción movimiento", MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Syncfusion.UI.Xaml.Grid.GridQueryableCollectionViewWrapper col = (Syncfusion.UI.Xaml.Grid.
                               GridQueryableCollectionViewWrapper)gvTransactions.View;
                    transactions = (Transactions)col.CurrentAddItem;

                    if(transactions != null)
                    {
                        if(transactions.date == null || transactions.accountid == null)
                        {
                            MessageBox.Show("Debe rellenar la fecha y cuenta para continuar", "Inserción movimiento");
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
            }

            frmSplits frm = new frmSplits(transactions);
            frm.ShowDialog();
            transactionsService.updateSplits(transactions);
            loadTransactions();
            refreshBalance();
        }

        private void gvTransactions_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
          
            if (MessageBox.Show("Esta seguro de querer eliminar este movimiento?", "Eliminación movimiento", MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
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

        private void gvTransactions_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Transactions transactions in e.Items)
            {
                removeTransaction(transactions);   
            }

            loadAccounts();
            loadTransactions();
            refreshBalance();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Implementar funcionalidad
            MessageBox.Show("Funcionalidad no implementada");
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (gvTransactions.SelectedItems != null && gvTransactions.SelectedItems.Count > 0)
            {
                foreach (Transactions transactions in gvTransactions.SelectedItems)
                {
                    removeTransaction(transactions);
                }
                loadTransactions();
                refreshBalance();
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
                refreshBalance();
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
                refreshBalance();
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
                refreshBalance();
            }
            else
            {
                MessageBox.Show("Tiene que seleccionar alguna línea.", "Cambio estado movimieno");
            }
        }

        private void gvTransactions_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (gvTransactions.CurrentItem != null)
            {
                frmTransaction frm = new frmTransaction((Transactions)gvTransactions.CurrentItem);
                frm.ShowDialog();
                loadAccounts();
                loadTransactions();
                refreshBalance();
            }
        }
    }
}
