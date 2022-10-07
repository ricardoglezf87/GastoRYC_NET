using GastosRYCLib.Manager;
using GastosRYCLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        private ICollectionView? viewTransaction;

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

        private void refreshBalance()
        {
            Decimal? balance = 0;

            if (viewTransaction != null)
            {
                foreach (Transactions t in from x in ((List<Transactions>)viewTransaction.SourceCollection)
                                           orderby x.orden ascending
                                           select x)
                {
                    if (lvCuentas.SelectedItem != null && ((Accounts)lvCuentas.SelectedItem).id == t.account?.id)
                    {
                        balance += t.amount;
                        t.balance = balance;
                    }
                    else if (lvCuentas.SelectedItem == null)
                    {
                        balance += t.amount;
                        t.balance = balance;
                    }

                }
            }

            gvMovimientos.ItemsSource = null;            
            gvMovimientos.ItemsSource = viewTransaction;

            viewTransaction?.SortDescriptions.Add(new SortDescription("orden", ListSortDirection.Ascending));
            viewTransaction?.Refresh();
        }

        private void frmInicio_Loaded(object sender, RoutedEventArgs e)
        {
            lvCuentas.ItemsSource = rycContext?.accounts?.ToList();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvCuentas.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("accountsTypes");
            view.GroupDescriptions.Add(groupDescription);

            cbAccounts.ItemsSource = rycContext?.accounts?.ToList();
            cbPersons.ItemsSource = rycContext?.persons?.ToList();
            cbCategories.ItemsSource = rycContext?.categories?.ToList();

            viewTransaction = CollectionViewSource.GetDefaultView(rycContext?.transactions?.ToList());
            refreshBalance();
        }

        public void ApplyFilters()
        {
            if (viewTransaction != null)
            {
                viewTransaction.Filter = accountFilter;
            }

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
        }

        private void GridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            lvCuentas.HorizontalContentAlignment = HorizontalAlignment.Stretch;
        }

        private void gvMovimientos_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
           //TODO: Hacer validaciones
        }

        private void gvMovimientos_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            Transactions t = (Transactions)e.Row.Item;
            t.orden = Double.Parse(t.date.Year.ToString("0000") 
                    + t.date.Month.ToString("00")
                    + t.date.Day.ToString("00") 
                    + t.id.ToString("000000")
                    + (t.amount < 0? "1": "0"));
            rycContext?.Update(t);
            rycContext?.SaveChangesAsync();
            refreshBalance();
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
                    t.orden = Double.Parse(date.Year.ToString("0000")
                            + date.Month.ToString("00")
                            + date.Day.ToString("00")
                            + t.id.ToString("000000")
                            + (t.amount < 0 ? "1" : "0"));
                    t.date = date;
                    rycContext?.Update(t);
                    rycContext?.SaveChangesAsync();
                    refreshBalance();
                }
            }
        }
    }
}
