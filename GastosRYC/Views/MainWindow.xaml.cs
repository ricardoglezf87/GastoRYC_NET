using BOLib.Services;
using BOLib.Models;
using GastosRYC.Views;
using GastosRYC.Views.Common;
using SimpleInjector;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using BOLib.ModelsView;

namespace GastosRYC
{
    public partial class MainWindow : RibbonWindow
    {

        #region Variables

        private ICollectionView? viewAccounts;
        private Page? actualPrincipalContent;
        private readonly AccountsService accountsService;
        private readonly DateCalendarService dateCalendarService;
        private readonly ExpirationsRemindersService expirationsRemindersService;
        private readonly InvestmentProductsPricesService investmentProductsPricesService;
        private readonly InvestmentProductsService investmentProductsService;


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
            accountsService = InstanceBase<AccountsService>.Instance;
            dateCalendarService = InstanceBase<DateCalendarService>.Instance;
            expirationsRemindersService = InstanceBase<ExpirationsRemindersService>.Instance;
            investmentProductsPricesService = InstanceBase<InvestmentProductsPricesService>.Instance;
            investmentProductsService = InstanceBase<InvestmentProductsService>.Instance;
        }

        #endregion

        #region Events

        private void btnUpdatePrices_Click(object sender, RoutedEventArgs e)
        {
            updatePrices();
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
            expirationsRemindersService.generateAutoregister();
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
                    ((PartialTransactions)actualPrincipalContent).ApplyFilters((AccountsView?)lvAccounts.SelectedValue);
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
            FrmAccountsList frm = new FrmAccountsList();
            frm.ShowDialog();
            loadAccounts();

            if (actualPrincipalContent is PartialTransactions)
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
        }

        private void btnMntPersons_Click(object sender, RoutedEventArgs e)
        {
            FrmPersonsList frm = new FrmPersonsList();
            frm.ShowDialog();

            if (actualPrincipalContent is PartialTransactions)
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
        }

        private void btnMntInvestmentProducts_Click(object sender, RoutedEventArgs e)
        {
            FrmInvestmentProductsList frm = new FrmInvestmentProductsList();
            frm.ShowDialog();

            if (actualPrincipalContent is PartialTransactions)
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
        }

        private void btnMntCategories_Click(object sender, RoutedEventArgs e)
        {
            FrmCategoriesList frm = new FrmCategoriesList();
            frm.ShowDialog();

            if (actualPrincipalContent is PartialTransactions)
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
        }

        private void btnMntTags_Click(object sender, RoutedEventArgs e)
        {
            FrmTagsList frm = new FrmTagsList();
            frm.ShowDialog();

            if (actualPrincipalContent is PartialTransactions)
                ((PartialTransactions)actualPrincipalContent).loadTransactions();

        }

        private void btnMntReminders_Click(object sender, RoutedEventArgs e)
        {
            FrmTransactionReminderList frm = new FrmTransactionReminderList();
            frm.ShowDialog();

            if (actualPrincipalContent is PartialReminders)
                ((PartialReminders)actualPrincipalContent).loadReminders();
        }


        #endregion

        #region Functions

        public void refreshBalance()
        {
            foreach (AccountsView accounts in lvAccounts.ItemsSource)
            {
                accounts.balance = accountsService.getBalanceByAccount(accounts.id);
            }

            autoResizeListView();
        }


        private void openNewTransaction()
        {
            FrmTransaction frm;

            if (lvAccounts.SelectedItem == null)
            {
                frm = new FrmTransaction();
            }
            else
            {
                frm = new FrmTransaction(((AccountsView)lvAccounts.SelectedItem).id);
            }

            frm.ShowDialog();
            loadAccounts();

            if (actualPrincipalContent is PartialTransactions)
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
        }

        private void loadCalendar()
        {
           dateCalendarService.fillCalendar();
        }

        private void toggleViews(eViews views)
        {
            Page? win = null;
            principalContent.Content = null;

            switch (views)
            {
                case eViews.Home:
                    lvAccounts.SelectedValue = null;
                    win = new PartialHome(this);
                    break;
                case eViews.Transactions:
                    win = new PartialTransactions(this);
                    break;
                case eViews.Reminders:
                    lvAccounts.SelectedValue = null;
                    win = new PartialReminders(this);
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
            viewAccounts = CollectionViewSource.GetDefaultView(accountsService.getAllOpenedListView());
            lvAccounts.ItemsSource = viewAccounts;
            viewAccounts.GroupDescriptions.Add(new PropertyGroupDescription("accountsTypesdescription"));
            viewAccounts.SortDescriptions.Add(new SortDescription("accountsTypesid", ListSortDirection.Ascending));
            refreshBalance();
        }

        private async void updatePrices()
        {
            try
            {
                List<InvestmentProducts>? lInvestmentProducts = investmentProductsService.getAll()?.Where(x => !String.IsNullOrWhiteSpace(x.url)).ToList();

                if (lInvestmentProducts != null)
                {
                    LoadDialog loadDialog = new(lInvestmentProducts.Count);
                    loadDialog.Show();

                    foreach (InvestmentProducts investmentProducts in lInvestmentProducts)
                    {
                        await investmentProductsPricesService.getPricesOnlineAsync(investmentProducts);
                        loadDialog.performeStep();
                    }

                    loadDialog.Close();
                    MessageBox.Show("Actualizado con exito!", "Actualización de precios");
                }
                else
                {
                    MessageBox.Show("No hay productos financieros a actualizar", "Actualización de precios");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha un ocurrido un error: " + ex.Message, "Actualización de precios");
            }
        }

        #endregion
    }
}
