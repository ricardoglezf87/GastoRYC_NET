using BBDDLib.Models;
using BBDDLib.Services;
using GastosRYC.BBDDLib.Services;
using GastosRYC.Views;
using SimpleInjector;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace GastosRYC
{
    public partial class MainWindow : RibbonWindow
    {

        #region Variables

        private ICollectionView? viewAccounts;
        private Page? actualPrincipalContent;
        private readonly SimpleInjector.Container servicesContainer;

        private enum eViews : int
        {
            Home = 1,
            Transactions = 2,
            Reminders = 3
        }

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            rbMenu.BackStageButton.Visibility = Visibility.Collapsed;
            SfSkinManager.ApplyStylesOnApplication = true;

            servicesContainer = new SimpleInjector.Container();
            registerServices();
        }

        #endregion

        #region Events


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

        private void btnAddTransaction_Click(object sender, RoutedEventArgs e)
        {
            openNewTransaction();
        }

        private void frmInicio_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    openNewTransaction();
                    break;
                case Key.F5:
                    if (actualPrincipalContent is PartialHome)
                    {
                        ((PartialHome)actualPrincipalContent).loadCharts();
                    }
                    else if (actualPrincipalContent is PartialTransactions)
                    {
                        loadAccounts();
                        ((PartialTransactions)actualPrincipalContent).loadTransactions();                        
                    }
                    else if (actualPrincipalContent is PartialReminders)
                    {
                        ((PartialReminders)actualPrincipalContent).loadReminders();
                    }
                    break;
            }
        }

        private void frmInicio_Loaded(object sender, RoutedEventArgs e)
        {
            loadCalendar();
            servicesContainer.GetInstance<ExpirationsRemindersService>().generateAutoregister();
            loadAccounts();
            toggleViews(eViews.Home);
        }

        private void lvAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvAccounts.SelectedValue != null)
            {
                toggleViews(eViews.Transactions);

                if (actualPrincipalContent is PartialReminders)
                    ((PartialReminders)actualPrincipalContent).loadReminders();

                if (actualPrincipalContent is PartialTransactions)
                    ((PartialTransactions)actualPrincipalContent).ApplyFilters((Accounts?)lvAccounts.SelectedValue);

            }
        }

        private void btnAllAccounts_Click(object sender, RoutedEventArgs e)
        {
            lvAccounts.SelectedItem = null;

            toggleViews(eViews.Transactions);

            if (actualPrincipalContent is PartialTransactions)
                ((PartialTransactions)actualPrincipalContent).ApplyFilters();
        }

        private void lvAccounts_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            autoResizeListView();
        }

        private void btnMntAccounts_Click(object sender, RoutedEventArgs e)
        {
            FrmAccountsList frm = new FrmAccountsList(servicesContainer);
            frm.ShowDialog();
            loadAccounts();

            if (actualPrincipalContent is PartialTransactions)
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
        }

        private void btnMntPersons_Click(object sender, RoutedEventArgs e)
        {
            FrmPersonsList frm = new FrmPersonsList(servicesContainer);
            frm.ShowDialog();

            if (actualPrincipalContent is PartialTransactions)
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
        }

        private void btnMntCategories_Click(object sender, RoutedEventArgs e)
        {
            FrmCategoriesList frm = new FrmCategoriesList(servicesContainer);
            frm.ShowDialog();

            if (actualPrincipalContent is PartialTransactions)
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
        }

        private void btnMntTags_Click(object sender, RoutedEventArgs e)
        {
            FrmTagsList frm = new FrmTagsList(servicesContainer);
            frm.ShowDialog();

            if (actualPrincipalContent is PartialTransactions)
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
            
        }

        private void btnMntReminders_Click(object sender, RoutedEventArgs e)
        {
            FrmTransactionReminderList frm = new FrmTransactionReminderList(servicesContainer);
            frm.ShowDialog();

            if (actualPrincipalContent is PartialReminders)
                ((PartialReminders)actualPrincipalContent).loadReminders();
        }


        #endregion

        #region Functions

        public void refreshBalance()
        {
            foreach (Accounts accounts in lvAccounts.ItemsSource)
            {
                accounts.balance = servicesContainer.GetInstance<TransactionsService>().getBalanceByAccount(accounts);
            }

            autoResizeListView();
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

            if (actualPrincipalContent is PartialTransactions)
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
        }

        private void loadCalendar()
        {
            servicesContainer.GetInstance<DateCalendarService>().fillCalendar();
        }

        private void registerServices()
        {
            servicesContainer.Register<AccountsService>(Lifestyle.Singleton);
            servicesContainer.Register<CategoriesService>(Lifestyle.Singleton);
            servicesContainer.Register<PersonsService>(Lifestyle.Singleton);
            servicesContainer.Register<TransactionsService>(Lifestyle.Singleton);
            servicesContainer.Register<SplitsService>(Lifestyle.Singleton);
            servicesContainer.Register<TagsService>(Lifestyle.Singleton);
            servicesContainer.Register<TransactionsRemindersService>(Lifestyle.Singleton);
            servicesContainer.Register<SplitsRemindersService>(Lifestyle.Singleton);
            servicesContainer.Register<ExpirationsRemindersService>(Lifestyle.Singleton);
            servicesContainer.Register<PeriodsRemindersService>(Lifestyle.Singleton);
            servicesContainer.Register<TransactionsStatusService>(Lifestyle.Singleton);
            servicesContainer.Register<CategoriesTypesService>(Lifestyle.Singleton);
            servicesContainer.Register<AccountsTypesService>(Lifestyle.Singleton);
            servicesContainer.Register<ForecastsChartService>(Lifestyle.Singleton);
            servicesContainer.Register<VBalancebyCategoryService>(Lifestyle.Singleton);
            servicesContainer.Register<DateCalendarService>(Lifestyle.Singleton);
        }

        private void toggleViews(eViews views)
        {
            Page? win = null;
            principalContent.Content = null;

            switch (views)
            {
                case eViews.Home:
                    lvAccounts.SelectedValue = null;
                    win = new PartialHome(servicesContainer, this);
                    break;
                case eViews.Transactions:
                    win = new PartialTransactions(servicesContainer, this);
                    break;
                case eViews.Reminders:
                    lvAccounts.SelectedValue = null;
                    win = new PartialReminders(servicesContainer, this);
                    break;
            }

            if (win != null)
            {
                actualPrincipalContent = win;
                principalContent.Content = win;
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

        public void loadAccounts()
        {
            viewAccounts = CollectionViewSource.GetDefaultView(servicesContainer.GetInstance<AccountsService>().getAllOpened());
            lvAccounts.ItemsSource = viewAccounts;
            viewAccounts.GroupDescriptions.Add(new PropertyGroupDescription("accountsTypes"));
            viewAccounts.SortDescriptions.Add(new SortDescription("accountsTypes.id", ListSortDirection.Ascending));
            refreshBalance();
        }

        #endregion

    }
}
