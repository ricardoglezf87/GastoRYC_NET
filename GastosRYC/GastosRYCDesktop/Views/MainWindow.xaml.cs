using GastosRYCLib.Manager;
using GastosRYCLib.Models;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Data.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GastosRYC
{
    public partial class MainWindow : Window
    {

        private RYCContext? rycContext;
        private ObservableCollection<Transactions>? viewTransaction;
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
                List<Accounts> lAccounts = (List<Accounts>) viewAccounts.SourceCollection;
                Accounts? accounts = lAccounts.FirstOrDefault(x => x.id == id);
                
                if (accounts != null)
                {
                    accounts.balance += balance;
                }
            }
        }

        private void refreshBalance()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Decimal? balanceTotal = 0;

                if (viewTransaction != null)
                {
                    reiniciarSaldosCuentas();

                    //gvMovimientos.SortColumnDescriptions.Clear();
                    //gvMovimientos.SortColumnDescriptions.Add(new Syncfusion.UI.Xaml.Grid.SortColumnDescription()
                    //{ ColumnName = "orden", SortDirection = ListSortDirection.Ascending });

                    //gvMovimientos.View.Refresh();

                    Syncfusion.UI.Xaml.Grid.
                        GridQueryableCollectionViewWrapper col = (Syncfusion.UI.Xaml.Grid.
                        GridQueryableCollectionViewWrapper) gvMovimientos.View;

                    foreach (Transactions t in col.ViewSource)
                    {
                        if (t.amount != null)
                        {
                            if (lvCuentas.SelectedItem != null && ((Accounts)lvCuentas.SelectedItem).id == t.account?.id)
                            {
                                balanceTotal += t.amount;
                            }
                            else if (lvCuentas.SelectedItem == null)
                            {
                                balanceTotal += t.amount;
                            }
                        }
                    }

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
                }


                //gvMovimientos.SortColumnDescriptions.Clear();
                //gvMovimientos.SortColumnDescriptions.Add(new Syncfusion.UI.Xaml.Grid.SortColumnDescription()
                //{ ColumnName = "orden", SortDirection = ListSortDirection.Descending });


                //viewAccounts?.Refresh();
                //gvMovimientos.View.Refresh();

                //gvMovimientos.ItemsSource = null;
                //gvMovimientos.ItemsSource = viewTransaction;

                //viewTransaction?.Refresh();
            }));
        }


        private void frmInicio_Loaded(object sender, RoutedEventArgs e)
        {
            viewAccounts = CollectionViewSource.GetDefaultView(rycContext?.accounts?.ToList());
            lvCuentas.ItemsSource = viewAccounts;
            viewAccounts.SortDescriptions.Add(new SortDescription("id",ListSortDirection.Ascending));
            viewAccounts.GroupDescriptions.Add(new PropertyGroupDescription("accountsTypes"));

            cbAccounts.ItemsSource = rycContext?.accounts?.ToList();

            cbPersons.ItemsSource = rycContext?.persons?.ToList();
           
            cbCategories.ItemsSource = rycContext?.categories?.ToList();

            if (rycContext?.transactions != null)
            {
                viewTransaction = new ObservableCollection<Transactions>(rycContext.transactions);
                gvMovimientos.ItemsSource = viewTransaction;
            }

            gvMovimientos.ItemsSource = viewTransaction;

            refreshBalance();
        }

        public void ApplyFilters()
        {
            gvMovimientos.View.Filter = accountFilter;
            gvMovimientos.View.RefreshFilter();
            Task.Factory.StartNew(refreshBalance);
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

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gvMovimientos != null && gvMovimientos.SelectedItem != null)
            {
                Transactions t = (Transactions)gvMovimientos.SelectedItem;
                DateTime date;
                DateTime.TryParse(e.Source.ToString(),out date);

                if (date != t.date)
                {
                    t.date = date;
                    saveChanges(t);
                    Task.Factory.StartNew(refreshBalance); 
                }
            }
        }

        private void gvMovimientos_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            saveChanges((Transactions)e.RowData);
            Task.Factory.StartNew(refreshBalance);   
        }

        private void saveChanges(Transactions transactions)
        {
            rycContext.Update(transactions);
            rycContext?.SaveChanges();
            gvMovimientos.View.Refresh();
        }
    }
}
