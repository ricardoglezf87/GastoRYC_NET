using GastosRYCLib.Manager;
using GastosRYCLib.Models;
using Microsoft.EntityFrameworkCore;
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

//TODO: quitar orden de la BBDD

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
            //Application.Current.Dispatcher.Invoke(new Action(() =>
            //{
            //    Decimal? balance = 0;

            //    if (viewTransaction != null)
            //    {
            //        reiniciarSaldosCuentas();

            //        foreach (Transactions t in from x in ((List<Transactions>)viewTransaction.SourceCollection)
            //                                   orderby x.orden ascending
            //                                   select x)
            //        {
            //            if (lvCuentas.SelectedItem != null && ((Accounts)lvCuentas.SelectedItem).id == t.account?.id)
            //            {
            //                balance += t.amount;
            //                t.balance = balance;
            //            }
            //            else if (lvCuentas.SelectedItem == null)
            //            {
            //                balance += t.amount;
            //                t.balance = balance;
            //            }

            //            if (t.amount != null)
            //                addSaldoCuenta(t.account?.id, t.amount.Value);

            //        }
            //    }

            //    viewAccounts?.Refresh();

            //    //gvMovimientos.ItemsSource = null;
            //    //gvMovimientos.ItemsSource = viewTransaction;
                
            //    //viewTransaction?.Refresh();
            //}));
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
                viewTransaction = new ObservableCollection<Transactions>(from x in rycContext.transactions
                                                                         select x);
                gvMovimientos.ItemsSource = CollectionViewSource.GetDefaultView(rycContext.transactions.ToList());
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
            //gvMovimientos.Columns.FirstOrDefault(x => x.MappingName == "account").IsHidden = true;            
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
                    //t.orden = Double.Parse(date.Year.ToString("0000")
                    //        + date.Month.ToString("00")
                    //        + date.Day.ToString("00")
                    //        + t.id.ToString("000000")
                    //        + (t.amount < 0 ? "1" : "0"));
                    t.date = date;
                    rycContext?.Update(t);
                    rycContext?.SaveChangesAsync();
                    refreshBalance();
                }
            }
        }

        private void gvMovimientos_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            //t.orden = Double.Parse(t.date.Year.ToString("0000")
            //        + t.date.Month.ToString("00")
            //        + t.date.Day.ToString("00")
            //        + t.id.ToString("000000")
            //        + (t.amount < 0 ? "1" : "0"));
            
            rycContext.Update((Transactions)e.RowData);
            rycContext?.SaveChanges();
            //Task.Factory.StartNew(refreshBalance);   
        }
    }
}
