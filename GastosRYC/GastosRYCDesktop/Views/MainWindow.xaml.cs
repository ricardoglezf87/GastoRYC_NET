using GastosRYCLib.Manager;
using GastosRYCLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Syncfusion.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GastosRYC
{
    public partial class MainWindow : Window
    {

        private RYCContext? rycContext;
        private ICollectionView? viewAccounts;

        public MainWindow()
        {
            InitializeComponent();
            loadContext();
        }

        private void loadContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            rycContext = new RYCContext();
            rycContext.categories?.Load();
            rycContext.persons?.Load();
            rycContext.accountsTypes?.Load();
            rycContext.accounts?.Load();
            rycContext.transactions?.Load();
        }

        private void reiniciarSaldosCuentas()
        {
            if (lvCuentas.ItemsSource != null)
            {
                foreach (Accounts accounts in lvCuentas.ItemsSource)
                {
                    accounts.balance = 0;
                }
            }
        }

        private void addSaldoCuenta(long? id, Decimal balance)
        {
            if (id != null && lvCuentas.ItemsSource != null
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

            if (gvMovimientos.View != null)
            {
                reiniciarSaldosCuentas();

                Syncfusion.UI.Xaml.Grid.
                    GridQueryableCollectionViewWrapper col = (Syncfusion.UI.Xaml.Grid.
                    GridQueryableCollectionViewWrapper)gvMovimientos.View;

                if (lvCuentas.SelectedItem != null)
                    balanceTotal = (decimal?)col.ViewSource.Where("accountid",
                        ((Accounts)lvCuentas.SelectedItem).id, Syncfusion.Data.FilterType.Equals, false).Sum("amount");
                else
                    balanceTotal = (decimal?)col.ViewSource.Sum("amount");

                foreach (Transactions t in col.ViewSource)
                {
                    if (t.amount != null)
                    {
                        if (lvCuentas.SelectedItem != null && ((Accounts)lvCuentas.SelectedItem).id == t.account?.id)
                        {
                            t.balance = balanceTotal;
                            balanceTotal -= t.amount;
                        }
                        else if (lvCuentas.SelectedItem == null)
                        {
                            t.balance = balanceTotal;
                            balanceTotal -= t.amount;
                        }

                        addSaldoCuenta(t.account?.id, t.amount.Value);
                    }
                }

                viewAccounts?.Refresh();
            }
        }

        private void frmInicio_Loaded(object sender, RoutedEventArgs e)
        {
            viewAccounts = CollectionViewSource.GetDefaultView(rycContext?.accounts?.ToList());
            lvCuentas.ItemsSource = viewAccounts;
            viewAccounts.SortDescriptions.Add(new SortDescription("id", ListSortDirection.Ascending));
            viewAccounts.GroupDescriptions.Add(new PropertyGroupDescription("accountsTypes"));

            cbAccounts.ItemsSource = rycContext?.accounts?.ToList();

            cbPersons.ItemsSource = rycContext?.persons?.ToList();

            cbCategories.ItemsSource = rycContext?.categories?.ToList();

            ObservableCollection<Transactions>? viewTransaction;

            if (rycContext?.transactions != null)
            {
                viewTransaction = new ObservableCollection<Transactions>(rycContext.transactions);
                gvMovimientos.ItemsSource = viewTransaction;
            }

            refreshBalance();
        }

        public void ApplyFilters()
        {
            gvMovimientos.View.Filter = accountFilter;
            gvMovimientos.View.RefreshFilter();
            refreshBalance();
        }

        public bool accountFilter(object? o)
        {
            Transactions? p = (o as Transactions);
            if (p == null)
                return false;
            else
                if (p.account?.id == ((Accounts)lvCuentas.SelectedValue)?.id)
                return true;
            else
                return false;
        }

        private void lvCuentas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
            
            gvMovimientos.Columns["accountid"].IsHidden = true;
        }

        private void GridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            lvCuentas.HorizontalContentAlignment = HorizontalAlignment.Stretch;
        }

        private void gvMovimientos_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {            
            saveChanges((Transactions)e.RowData);
            refreshBalance();
        }

        private void saveChanges(Transactions transactions)
        {
            rycContext.Update(transactions);          
            rycContext?.SaveChanges();
            gvMovimientos.View.Refresh();
        }

        private void gvMovimientos_AddNewRowInitiating(object sender, Syncfusion.UI.Xaml.Grid.AddNewRowInitiatingEventArgs e)
        {
            var data = e.NewObject as Transactions;
            data.date = DateTime.Now;

            if (lvCuentas.SelectedItem != null)
            {
                data.accountid = (lvCuentas.SelectedItem as Accounts).id;
            }
        }

        private void gvMovimientos_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
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
            
            if (transactions.amount == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("amount", "Tiene que rellenar la cantidad");
            }
        }
    }
}
