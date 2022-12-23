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
using System.Windows.Input;
using Syncfusion.UI.Xaml.Charts;
using Syncfusion.XPS;
using System.Drawing;
using System.Runtime.InteropServices;
using BBDDLib.Models.Charts;
using System.Windows.Media;
using System.Windows.Media.Effects;

//TODO: implementar split

namespace GastosRYC
{
    public partial class MainWindow : Window
    {

        #region Variables

        private ICollectionView? viewAccounts;

        private readonly AccountsService accountsService = new AccountsService();
        private readonly TransactionsService transactionsService = new TransactionsService();
        private readonly SplitsService splitsService = new SplitsService();

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            gridHome.Visibility = Visibility.Visible;
            gvTransactions.Visibility = Visibility.Hidden;
            loadCharts();
        }

        private void btnNewTransaction_Click(object sender, RoutedEventArgs e)
        {
            openNewTransaction();
        }

        private void MenuItem_NewTransaction_Click(object sender, RoutedEventArgs e)
        {
            openNewTransaction();
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


        private void frmInicio_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    openNewTransaction();
                    break;
                case Key.F5:
                    loadAccounts();
                    loadTransactions();
                    refreshBalance();
                    loadCharts();
                    break;
            }
        }

        private void frmInicio_Loaded(object sender, RoutedEventArgs e)
        {
            loadAccounts();
            loadTransactions();            
            refreshBalance();
            loadCharts();
        }



        private void lvAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
            gvTransactions.Columns["account.description"].IsHidden = true;
            gridHome.Visibility = Visibility.Hidden;
            gvTransactions.Visibility = Visibility.Visible;
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


        private void btnAllAccounts_Click(object sender, RoutedEventArgs e)
        {
            lvAccounts.SelectedItem = null;
            gvTransactions.Columns["account.description"].IsHidden = false;
            gridHome.Visibility = Visibility.Hidden;
            gvTransactions.Visibility = Visibility.Visible;
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

        #endregion

        #region Functions

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

        private void loadCharts()
        {   
            //Header

            Border border = new Border()
            {

                BorderThickness = new Thickness(0.5),

                BorderBrush = new System.Windows.Media.SolidColorBrush(Colors.Black),

                Margin = new Thickness(10),

                CornerRadius = new CornerRadius(5)

            };

            TextBlock textBlock = new TextBlock()
            {

                Text = "Clasificación Gastos",

                Margin = new Thickness(5),

                FontSize = 14

            };

            textBlock.Effect = new DropShadowEffect()
            {

                Color = Colors.Black,

                Opacity = 0.5

            };

            border.Child = textBlock;

            chExpenses.Header = border;

            //Axis

            CategoryAxis primaryAxis = new CategoryAxis();
            primaryAxis.Header = "Categoría";
            chExpenses.PrimaryAxis = primaryAxis;

            NumericalAxis secondaryAxis = new NumericalAxis();
            secondaryAxis.Header = "Importe (€)";
            chExpenses.SecondaryAxis = secondaryAxis;

            //ToolTip
           
            DataTemplate tooltip = new DataTemplate();
            
            FrameworkElementFactory stackpanel = new FrameworkElementFactory(typeof(StackPanel));
            stackpanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            FrameworkElementFactory textblock = new FrameworkElementFactory(typeof(TextBlock));
            textblock.SetBinding(TextBlock.TextProperty, new Binding("Item.category"));
            textblock.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textblock.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textblock.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            textblock.SetValue(TextBlock.ForegroundProperty, new System.Windows.Media.SolidColorBrush(Colors.Black));

            stackpanel.AppendChild(textblock);

            FrameworkElementFactory textblock1 = new FrameworkElementFactory(typeof(TextBlock));
            textblock1.SetValue(TextBlock.TextProperty, " : ");
            textblock1.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textblock1.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textblock1.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            textblock1.SetValue(TextBlock.ForegroundProperty, new System.Windows.Media.SolidColorBrush(Colors.Black));

            stackpanel.AppendChild(textblock1);

            FrameworkElementFactory textblock2 = new FrameworkElementFactory(typeof(TextBlock));
            textblock2.SetBinding(TextBlock.TextProperty, 
                new Binding("Item.amount") { 
                    StringFormat = "C",
                    ConverterCulture = new System.Globalization.CultureInfo("es-ES") 
                });
            
            textblock2.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            textblock2.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textblock2.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            textblock2.SetValue(TextBlock.ForegroundProperty, new System.Windows.Media.SolidColorBrush(Colors.Black));
           
            stackpanel.AppendChild(textblock2);
            tooltip.VisualTree = stackpanel;

            //Series

            List<ExpensesChart> lExpensesCharts = transactionsService.getExpenses();
            chExpenses.Series.Clear();

            ColumnSeries series = new ColumnSeries()
            {
                ItemsSource = lExpensesCharts.OrderByDescending(x=>x.amount).Take(10),
                XBindingPath = "category",
                YBindingPath = "amount",
                ShowTooltip = true,
                TooltipTemplate = tooltip,
                EnableAnimation = true,
                AnimationDuration = new TimeSpan(0,0,3)
            };

            ChartTooltip.SetShowDuration(series,5000);
            chExpenses.Series.Add(series);

            //Grid

            gvExpenses.ItemsSource = lExpensesCharts.OrderByDescending(x => x.amount);

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
        private void openNewTransaction()
        {
            frmTransaction frm;

            if (lvAccounts.SelectedItem == null)
            {
                frm = new frmTransaction();
            }
            else
            {
                frm = new frmTransaction(((Accounts)lvAccounts.SelectedItem).id);
            }

            frm.ShowDialog();
            loadAccounts();
            loadTransactions();
            refreshBalance();
        }

        #endregion

        
    }
}
