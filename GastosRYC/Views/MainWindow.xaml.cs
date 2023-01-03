using BBDDLib.Models;
using BBDDLib.Models.Charts;
using BBDDLib.Services.Interfaces;
using GastosRYC.BBDDLib.Services;
using GastosRYC.Extensions;
using GastosRYC.Views;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace GastosRYC
{
    public partial class MainWindow : Window
    {

        #region Variables

        private ICollectionView? viewAccounts;
        private readonly SimpleInjector.Container servicesContainer;

        private enum eViews : int
        {
            Home = 1,
            Transactions = 2,
            Reminders = 3
        }

        private eViews activeView = eViews.Home;

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            servicesContainer = new SimpleInjector.Container();
            registerServices();
        }

        #endregion

        #region Events

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer registrar este recordatorío?", "recordatorio movimiento", MessageBoxButton.YesNo,
               MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                if (sender != null && ((Button)sender)?.Tag != null)
                {
                    makeTransactionFromReminder((int?)((Button)sender).Tag);
                    putDoneReminder((int?)((Button)sender).Tag);
                }
            }
        }

        private void btnSkip_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer saltar este recordatorío?", "recordatorio movimiento", MessageBoxButton.YesNo,
                   MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                if (sender != null && ((Button)sender)?.Tag != null)
                {
                    putDoneReminder((int?)((Button)sender).Tag);
                }
            }
        }

        private void btnReminders_Click(object sender, RoutedEventArgs e)
        {
            toggleViews(eViews.Reminders);
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            toggleViews(eViews.Home);
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
                    transactions.transactionStatusid = (int)ITransactionsStatusService.eTransactionsTypes.Pending;
                    servicesContainer.GetInstance<ITransactionsService>().update(transactions);
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
                    transactions.transactionStatusid = (int)ITransactionsStatusService.eTransactionsTypes.Provisional;
                    servicesContainer.GetInstance<ITransactionsService>().update(transactions);
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
                    transactions.transactionStatusid = (int)ITransactionsStatusService.eTransactionsTypes.Reconciled;
                    servicesContainer.GetInstance<ITransactionsService>().update(transactions);
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
                FrmTransaction frm = new FrmTransaction((Transactions)gvTransactions.CurrentItem, servicesContainer);
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
                    switch (activeView)
                    {
                        case eViews.Transactions:
                            loadAccounts();
                            loadTransactions();
                            refreshBalance();
                            break;
                        case eViews.Reminders:
                            loadReminders();
                            break;
                        case eViews.Home:
                            loadCharts();
                            break;
                    }
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
            if (lvAccounts.SelectedValue != null)
            {
                ApplyFilters();
                gvTransactions.Columns["account.description"].IsHidden = true;
                toggleViews(eViews.Transactions);
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
                    transactionsReminders.transactionStatusid = (int)ITransactionsStatusService.eTransactionsTypes.Pending;

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


        private void btnAllAccounts_Click(object sender, RoutedEventArgs e)
        {
            lvAccounts.SelectedItem = null;
            gvTransactions.Columns["account.description"].IsHidden = false;
            toggleViews(eViews.Transactions);
        }

        private void lvAccounts_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            autoResizeListView();
        }

        private void MenuItem_Accounts_Click(object sender, RoutedEventArgs e)
        {
            FrmAccountsList frm = new FrmAccountsList(servicesContainer);
            frm.ShowDialog();
            loadAccounts();
            loadTransactions();
            refreshBalance();
        }

        private void MenuItem_Persons_Click(object sender, RoutedEventArgs e)
        {
            FrmPersonsList frm = new FrmPersonsList(servicesContainer);
            frm.ShowDialog();
            loadTransactions();
            refreshBalance();
        }

        private void MenuItem_Categories_Click(object sender, RoutedEventArgs e)
        {
            FrmCategoriesList frm = new FrmCategoriesList(servicesContainer);
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
            FrmTagsList frm = new FrmTagsList(servicesContainer);
            frm.ShowDialog();
            loadTransactions();
            refreshBalance();
        }

        private void MenuItem_Reminders_Click(object sender, RoutedEventArgs e)
        {
            FrmTransactionReminderList frm = new FrmTransactionReminderList(servicesContainer);
            frm.ShowDialog();
            loadReminders();
        }

        private void ButtonSplit_Click(object sender, RoutedEventArgs e)
        {
            Transactions transactions = (Transactions)gvTransactions.SelectedItem;
            FrmSplitsList frm = new FrmSplitsList(transactions, servicesContainer);
            frm.ShowDialog();
            servicesContainer.GetInstance<ITransactionsService>().updateTransactionAfterSplits(transactions);
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

        private void cvReminders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (cvReminders.SelectedItem != null && ((ExpirationsReminders)cvReminders.SelectedItem).transactionsReminders != null)
            {
                FrmTransactionReminders frm = new FrmTransactionReminders(((ExpirationsReminders)cvReminders.SelectedItem).transactionsReminders, servicesContainer);
                frm.ShowDialog();
                loadReminders();
            }
        }

        #endregion

        #region Functions

        private void registerServices()
        {
            servicesContainer.Register<IAccountsService, AccountsService>();
            servicesContainer.Register<ICategoriesService, CategoriesService>();
            servicesContainer.Register<IPersonsService, PersonsService>();
            servicesContainer.Register<ITransactionsService, TransactionsService>();
            servicesContainer.Register<ISplitsService, SplitsService>();
            servicesContainer.Register<ITagsService, TagsService>();
            servicesContainer.Register<ITransactionsRemindersService, TransactionsRemindersService>();
            servicesContainer.Register<ISplitsRemindersService, SplitsRemindersService>();
            servicesContainer.Register<IExpirationsRemindersService, ExpirationsRemindersService>();
            servicesContainer.Register<IPeriodsRemindersService, PeriodsRemindersService>();
            servicesContainer.Register<ITransactionsStatusService, TransactionsStatusService>();
            servicesContainer.Register<ICategoriesTypesService, CategoriesTypesService>();
            servicesContainer.Register<IAccountsTypesService, AccountsTypesService>();
            servicesContainer.Register<IChartsService, ChartsService>();
        }

        private void toggleViews(eViews views)
        {
            gridTransactions.Visibility = Visibility.Hidden;
            gridHome.Visibility = Visibility.Hidden;
            gridReminders.Visibility = Visibility.Hidden;

            activeView = views;

            switch (views)
            {
                case eViews.Home:
                    gridHome.Visibility = Visibility.Visible;
                    loadCharts();
                    lvAccounts.SelectedValue = null;
                    break;
                case eViews.Transactions:
                    gridTransactions.Visibility = Visibility.Visible;
                    loadTransactions();
                    refreshBalance();
                    break;
                case eViews.Reminders:
                    gridReminders.Visibility = Visibility.Visible;
                    loadReminders();
                    lvAccounts.SelectedValue = null;
                    break;
            }


        }

        private void loadReminders()
        {
            //TODO: Evitar error al volver al cargar que no agrupa, ahora no esta del todo como me gustaria         

            if (cvReminders.ItemsSource == null)
            {
                cvReminders.ItemsSource = new ListCollectionView(servicesContainer.GetInstance<IExpirationsRemindersService>().getAllPendingWithoutFutureWithGeneration());

                cvReminders.CanGroup = true;
                cvReminders.GroupCards("groupDate");

                cvReminders.Items.SortDescriptions.Clear();
                cvReminders.Items.SortDescriptions.Add(
                    new System.ComponentModel.SortDescription("date", System.ComponentModel.ListSortDirection.Ascending));
            }
            else
            {
                while (((ListCollectionView)cvReminders.ItemsSource).Count > 0)
                {
                    ((ListCollectionView)cvReminders.ItemsSource).RemoveAt(0);
                }

                foreach (ExpirationsReminders expirationsReminders in servicesContainer.GetInstance<IExpirationsRemindersService>().getAllPendingWithoutFutureWithGeneration())
                {
                    ((ListCollectionView)cvReminders.ItemsSource).AddNewItem(expirationsReminders);
                    ((ListCollectionView)cvReminders.ItemsSource).CommitNew();
                }
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

        private void loadCharts()
        {
            loadChartForecast();
            loadChartExpenses();
        }

        private void loadChartForecast()
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

                Text = "Prevision de cobros / pagos",

                Margin = new Thickness(5),

                FontSize = 14

            };

            textBlock.Effect = new DropShadowEffect()
            {

                Color = Colors.Black,

                Opacity = 0.5

            };

            border.Child = textBlock;

            chForecast.Header = border;

            //Legend

            chForecast.Legend = new ChartLegend()
            {
                IconHeight = 10,
                IconWidth = 10,
                Margin = new Thickness(0, 0, 0, 5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                DockPosition = ChartDock.Right,
                IconVisibility = Visibility.Visible,
                CornerRadius = new CornerRadius(5),
                ItemMargin = new Thickness(10),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                CheckBoxVisibility = Visibility.Visible
            };

            //Axis

            DateTimeAxis primaryAxis = new DateTimeAxis();
            primaryAxis.Header = "Fecha";
            primaryAxis.Minimum = DateTime.Today.AddDays(-1);
            primaryAxis.Maximum= DateTime.Today.AddMonths(1).AddDays(1);
            primaryAxis.IntervalType = DateTimeIntervalType.Days;
            primaryAxis.Interval = 1;
            primaryAxis.LabelFormat = "dd/mm";
            chForecast.PrimaryAxis = primaryAxis;

            NumericalAxis secondaryAxis = new NumericalAxis();
            secondaryAxis.Header = "Importe (€)";
            chForecast.SecondaryAxis = secondaryAxis;

            //ToolTip

            DataTemplate tooltip = new DataTemplate();

            FrameworkElementFactory stackpanel = new FrameworkElementFactory(typeof(StackPanel));
            stackpanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            FrameworkElementFactory textblock = new FrameworkElementFactory(typeof(TextBlock));
            textblock.SetBinding(TextBlock.TextProperty, new Binding("Item.account"));
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
                new Binding("Item.amount")
                {
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

            chForecast.Series.Clear();

            foreach (Accounts accounts in servicesContainer.GetInstance<IAccountsService>().getAll()?
                .Where(x=> servicesContainer.GetInstance<IAccountsTypesService>().accountExpensives(x.accountsTypesid)))
            {

                SplineSeries series = new SplineSeries()
                {
                    ItemsSource = servicesContainer.GetInstance<IChartsService>().getMonthForecast()
                        .Where(x=> x.accountid == accounts.id).OrderByDescending(x => x.date),
                    Label = accounts.description,
                    XBindingPath = "date",
                    YBindingPath = "amount",                    
                    ShowTooltip = true,
                    TooltipTemplate = tooltip,
                    EnableAnimation = true,
                    AnimationDuration = new TimeSpan(0, 0, 3),
                    AdornmentsInfo = new ChartAdornmentInfo()
                    {
                        ShowMarker = true,
                        SymbolStroke = new SolidColorBrush(Colors.Blue),
                        SymbolInterior = new SolidColorBrush(Colors.DarkBlue),
                        SymbolHeight = 10,
                        SymbolWidth = 10,
                        Symbol = ChartSymbol.Ellipse
                    }
                };


                ChartTooltip.SetShowDuration(series, 5000);
                chForecast.Series.Add(series);
            }
        }

        private void loadChartExpenses()
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
                new Binding("Item.amount")
                {
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

            List<ExpensesChart> lExpensesCharts = servicesContainer.GetInstance<IChartsService>().getExpenses();
            chExpenses.Series.Clear();

            ColumnSeries series = new ColumnSeries()
            {
                ItemsSource = lExpensesCharts.OrderByDescending(x => x.amount).Take(10),
                XBindingPath = "category",
                YBindingPath = "amount",
                ShowTooltip = true,
                TooltipTemplate = tooltip,
                EnableAnimation = true,
                AnimationDuration = new TimeSpan(0, 0, 3)
            };

            ChartTooltip.SetShowDuration(series, 5000);
            chExpenses.Series.Add(series);

            //Grid

            gvExpenses.ItemsSource = lExpensesCharts.OrderByDescending(x => x.amount);
        }

        private void loadAccounts()
        {
            viewAccounts = CollectionViewSource.GetDefaultView(servicesContainer.GetInstance<IAccountsService>().getAll());
            lvAccounts.ItemsSource = viewAccounts;
            viewAccounts.GroupDescriptions.Add(new PropertyGroupDescription("accountsTypes"));
            viewAccounts.SortDescriptions.Add(new SortDescription("accountsTypes.id", ListSortDirection.Ascending));
        }

        private void loadTransactions()
        {
            gvTransactions.ItemsSource = servicesContainer.GetInstance<ITransactionsService>().getAll();
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
                            servicesContainer.GetInstance<ITransactionsService>().delete(servicesContainer.GetInstance<ITransactionsService>().getByID(splits.tranferid));
                        }

                        servicesContainer.GetInstance<ISplitsService>().delete(splits);
                    }
                }

                if (transactions.tranferid != null)
                {
                    servicesContainer.GetInstance<ITransactionsService>().delete(servicesContainer.GetInstance<ITransactionsService>().getByID(transactions.tranferid));
                }

                servicesContainer.GetInstance<ITransactionsService>().delete(transactions);
            }
        }
        private void openNewTransaction()
        {
            FrmTransaction frm;

            if (lvAccounts.SelectedItem == null)
            {
                frm = new FrmTransaction(servicesContainer);
            }
            else
            {
                frm = new FrmTransaction(((Accounts)lvAccounts.SelectedItem).id, servicesContainer);
            }

            frm.ShowDialog();
            loadAccounts();
            loadTransactions();
            refreshBalance();
        }

        private void makeTransactionFromReminder(int? id)
        {
            servicesContainer.GetInstance<IExpirationsRemindersService>().registerTransactionfromReminder(id);

            loadTransactions();
            refreshBalance();
            loadAccounts();
        }

        private void putDoneReminder(int? id)
        {
            ExpirationsReminders? expirationsReminders = servicesContainer.GetInstance<IExpirationsRemindersService>().getByID(id);
            if (expirationsReminders != null)
            {
                expirationsReminders.done = true;
                servicesContainer.GetInstance<IExpirationsRemindersService>().update(expirationsReminders);
            }

            loadReminders();
        }

        #endregion

    }
}
