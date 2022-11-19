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

//TODO: implementar split

namespace GastosRYC
{
    public partial class MainWindow : Window
    {
        private ICollectionView? viewAccounts;
        private Boolean needRefresh;
        private DispatcherTimer? dispatcherTimer;

        private readonly AccountsService accountsService = new AccountsService();        
        private readonly CategoriesService categoriesService = new CategoriesService();
        private readonly PersonsService personService = new PersonsService();
        private readonly TagsService tagsService = new TagsService();
        private readonly TransactionsService transactionsService = new TransactionsService();
        private readonly SplitsService splitsService = new SplitsService();
        private readonly TransactionsStatusService transactionsStatusService = new TransactionsStatusService();

        public MainWindow()
        {
            InitializeComponent();
            loadTimer();
        }

        private void loadTimer()
        {
            needRefresh = false;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object? sender, EventArgs e)
        {
            if (needRefresh)
            {
                refreshBalance();
                needRefresh = false;
            }
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
            needRefresh = true;
        }

        private void loadComboBox()
        {
            cbAccounts.ItemsSource = accountsService.getAll();
            cbPersons.ItemsSource = personService.getAll();
            cbCategories.ItemsSource = categoriesService.getAll();
            cbTags.ItemsSource = tagsService.getAll();
            cbTransStatus.ItemsSource = transactionsStatusService.getAll();       
        }

        private void loadTransactions()
        {
            gvTransactions.ItemsSource = transactionsService.getAll();
            ApplyFilters();
            needRefresh = true;
        }

        private void frmInicio_Loaded(object sender, RoutedEventArgs e)
        {
            loadAccounts();
            loadComboBox();
            loadTransactions();       
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
            needRefresh = true;
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
            gvTransactions.Columns["accountid"].IsHidden = true;
        }


        private void btnAllAccounts_Click(object sender, RoutedEventArgs e)
        {
            lvAccounts.SelectedItem = null;
            gvTransactions.Columns["accountid"].IsHidden = false;
        }

        private void gvTransactions_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            saveChanges((Transactions)e.RowData);
            needRefresh = true;
        }

        private void saveChanges(Transactions transactions)
        {
            if (transactions.account == null && transactions.accountid != null)
            {
                transactions.account = accountsService.getByID(transactions.accountid);
                transactions.person = personService.getByID(transactions.personid);
                transactions.category = categoriesService.getByID(transactions.categoryid);
                transactions.tag = tagsService.getByID(transactions.tagid);
                transactions.transactionStatus = transactionsStatusService.getByID(transactions.transactionStatusid);
            }

            if (transactions.amountIn == null)
                transactions.amountIn = 0;

            if (transactions.amountOut == null)
                transactions.amountOut = 0;

            updateTranfer(transactions);
            updateTranferSplit(transactions);
            transactionsService.update(transactions);

            loadTransactions();
           
        }

        private void updateTranfer(Transactions transactions)
        {
            if (transactions.tranferid != null && 
                transactions.category.categoriesTypesid != (int)CategoriesService.eCategoriesTypes.Transferencias)
            {
                Transactions? tContraria = transactionsService.getByID(transactions.tranferid);
                if (tContraria != null)
                {
                    transactionsService.delete(tContraria);
                }
                transactions.tranferid = null;
            }
            else if (transactions.tranferid == null && 
                transactions.category.categoriesTypesid == (int)CategoriesService.eCategoriesTypes.Transferencias)
            {
                transactions.tranferid = transactionsService.getNextID();

                Transactions? tContraria = new Transactions();
                tContraria.date = transactions.date;
                tContraria.accountid = transactions.category.accounts.id;
                tContraria.personid = transactions.personid;
                tContraria.categoryid = transactions.account.categoryid;
                tContraria.memo = transactions.memo;
                tContraria.tagid = transactions.tagid;
                tContraria.amountIn = transactions.amountOut;
                tContraria.amountOut = transactions.amountIn;

                if (transactions.id != 0)
                    tContraria.tranferid = transactions.id;
                else
                    tContraria.tranferid = transactionsService.getNextID() + 1;

                tContraria.transactionStatusid = transactions.transactionStatusid;

                transactionsService.update(tContraria);

            }
            else if (transactions.tranferid != null && 
                transactions.category.categoriesTypesid == (int)CategoriesService.eCategoriesTypes.Transferencias)
            {
                Transactions? tContraria = transactionsService.getByID(transactions.tranferid);
                if (tContraria != null)
                {
                    tContraria.date = transactions.date;
                    tContraria.accountid = transactions.category.accounts.id;
                    tContraria.personid = transactions.personid;
                    tContraria.categoryid = transactions.account.categoryid;
                    tContraria.memo = transactions.memo;
                    tContraria.tagid = transactions.tagid;
                    tContraria.amountIn = transactions.amountOut;
                    tContraria.amountOut = transactions.amountIn;
                    tContraria.transactionStatusid = transactions.transactionStatusid;
                    transactionsService.update(tContraria);
                }
            }
        }

        private void updateTranferSplit(Transactions transactions)
        {
            if (transactions.tranferSplitid != null && 
                transactions.category.categoriesTypesid == (int)CategoriesService.eCategoriesTypes.Transferencias)
            {
                Splits? tContraria = splitsService.getByID(transactions.tranferSplitid);
                if (tContraria != null)
                {
                    tContraria.transaction.date = transactions.date;                    
                    tContraria.transaction.personid = transactions.personid;
                    tContraria.categoryid = transactions.account.categoryid;
                    tContraria.memo = transactions.memo;
                    tContraria.tagid = transactions.tagid;
                    tContraria.amountIn = transactions.amountOut;
                    tContraria.amountOut = transactions.amountIn;
                    tContraria.transaction.transactionStatusid = transactions.transactionStatusid;
                    splitsService.update(tContraria);
                }
            }
        }

        private void gvTransactions_AddNewRowInitiating(object sender, Syncfusion.UI.Xaml.Grid.AddNewRowInitiatingEventArgs e)
        {
            var data = e.NewObject as Transactions;
            data.date = DateTime.Now;
            
            if (lvAccounts.SelectedItem != null)
            {
                data.accountid = (lvAccounts.SelectedItem as Accounts).id;
            }

            data.transactionStatusid = transactionsStatusService.getFirst()?.id;
        }

        private void gvTransactions_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            Transactions transactions = (Transactions)e.RowData;

            if (transactions.date == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("date", "Tiene que rellenar la fecha");
            }

            if (transactions.accountid == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("accountid", "Tiene que rellenar la cuenta");
            }

            if (transactions.categoryid == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("categoryid", "Tiene que rellenar la categoria");
            }
            else
            {

                if (transactions.categoryid == (int)CategoriesService.eSpecialCategories.Split &&
                    (transactions.splits == null || transactions.splits.Count == 0))
                {
                    e.IsValid = false;
                    e.ErrorMessages.Add("categoryid", "No se puede utilizar la categoria Split si no se tiene desglose de movimiento");
                }
                else if (transactions.categoryid != (int)CategoriesService.eSpecialCategories.Split &&
                    transactions.splits != null && transactions.splits.Count > 0)
                {
                    e.IsValid = false;
                    e.ErrorMessages.Add("categoryid", "No se puede utilizar la categoria distinta de Split si se tiene desglose de movimiento");
                }
                else if (transactions.tranferSplitid != null)
                {
                    e.IsValid = false;
                    e.ErrorMessages.Add("categoryid", "No se puede cambiar el movimiento cuando la transferencia proviene de un split.");
                    e.ErrorMessages.Add("amountIn", "No se puede cambiar el movimiento cuando la transferencia proviene de un split.");
                    e.ErrorMessages.Add("amountOut", "No se puede cambiar el movimiento cuando la transferencia proviene de un split.");
                }
            }

            if (transactions.amountIn == null && transactions.amountOut == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("amountIn", "Tiene que rellenar la cantidad");
                e.ErrorMessages.Add("amountOut", "Tiene que rellenar la cantidad");
            }
            else if (transactions.splits != null && transactions.splits.Count > 0
                && splitsService.getAmountTotal(transactions) != transactions.amount)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("amountIn", "No se puede cambiar las cantidades cuando el movimiento es un split.");
                e.ErrorMessages.Add("amountOut", "No se puede cambiar las cantidades cuando el movimiento es un split.");
            }

            if (transactions.transactionStatusid == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("transactionStatusid", "Tiene que rellenar el estado");
            }
        }

        private void lvAccounts_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            autoResizeListView();
        }

        private void gvTransactions_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            Transactions transactions = (Transactions)gvTransactions.SelectedItem;
            if (transactions != null)
            {
                switch (gvTransactions.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "accountid":
                        transactions.account = accountsService.getByID(transactions.accountid);
                        break;
                    case "personid":
                        transactions.person = personService.getByID(transactions.personid);
                        break;
                    case "categoryid":
                        transactions.category = categoriesService.getByID(transactions.categoryid);
                        break;
                    case "tagid":
                        transactions.tag = tagsService.getByID(transactions.tagid);
                        break;
                    case "transactionStatusid":
                        transactions.transactionStatus = transactionsStatusService.getByID(transactions.transactionStatusid);
                        break;
                }
            }
        }

        private void MenuItem_Accounts_Click(object sender, RoutedEventArgs e)
        {
            frmAccounts frm = new frmAccounts();
            frm.ShowDialog();
            loadAccounts();
            loadComboBox();
            loadTransactions();
        }

        private void MenuItem_Persons_Click(object sender, RoutedEventArgs e)
        {
            frmPersons frm = new frmPersons();
            frm.ShowDialog();
            loadComboBox();
            loadTransactions();
        }

        private void MenuItem_Categories_Click(object sender, RoutedEventArgs e)
        {
            frmCategories frm = new frmCategories();
            frm.ShowDialog();
            loadComboBox();
            loadTransactions();
        }

        private void MenuItem_Salir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Tags_Click(object sender, RoutedEventArgs e)
        {
            frmTags frm = new frmTags();
            frm.ShowDialog();
            loadComboBox();
            loadTransactions();
        }

        private void updateSplits(Transactions transactions)
        {
            if (transactions.splits != null && transactions.splits.Count != 0)
            {
                transactions.amountIn = 0;
                transactions.amountOut = 0;

                foreach (Splits splits in transactions.splits)
                {
                    transactions.amountIn += (splits.amountIn == null ? 0 : splits.amountIn);
                    transactions.amountOut += (splits.amountOut == null ? 0 : splits.amountOut);
                }

                transactions.categoryid = (int)CategoriesService.eSpecialCategories.Split;
                transactions.category = categoriesService.getByID((int)CategoriesService.eSpecialCategories.Split);
            }
            else if(transactions.categoryid != null
                && transactions.categoryid == (int)CategoriesService.eSpecialCategories.Split)
            {
                transactions.categoryid = (int)CategoriesService.eSpecialCategories.SinCategoria;
                transactions.category = categoriesService.getByID((int)CategoriesService.eSpecialCategories.SinCategoria);
            }


            transactionsService.update(transactions);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Transactions transactions = (Transactions)gvTransactions.SelectedItem;
            frmSplits frm = new frmSplits(transactions);
            frm.ShowDialog();
            updateSplits(transactions);
            loadTransactions();
        }

        private void gvTransactions_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            foreach (Transactions transactions in e.Items)
            {
                if (transactions.tranferSplitid != null)
                {
                    MessageBox.Show("No se puede borrar un movimiento que venga de una transferencia de split","Eliminación movimiento");
                    e.Cancel = true;

                }
            }

            if (!e.Cancel && MessageBox.Show("Esta seguro de querer eliminar este movimiento?", "Eliminación movimiento", MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void gvTransactions_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Transactions transactions in e.Items)
            {
                if (transactions.splits != null)
                {
                    List<Splits> lSplits = transactions.splits;
                    for (int i=0; i < lSplits.Count; i++)
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

                loadAccounts();
                loadTransactions();
            }
        }
    }
}
