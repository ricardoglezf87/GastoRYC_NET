using GARCA.BO.Models;
using GARCA.BO.ModelsView;
using GARCA.BO.Services;
using GARCA.IOC;
using GARCA.Views;
using GARCA.Views.Common;
using GARCA.WebReport;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace GARCA
{
    public partial class MainWindow : RibbonWindow
    {

        #region Variables

        private ICollectionView? viewAccounts;
        private Page? actualPrincipalContent;

        private enum eViews : int
        {
            Home = 1,
            Transactions = 2,
            Reminders = 3,
            Portfolio = 4
        }

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            rbMenu.BackStageButton.Visibility = Visibility.Collapsed;
            SfSkinManager.ApplyStylesOnApplication = true;

            DependencyConfig.iRYCContextService.makeBackup();
            DependencyConfig.iRYCContextService.migrateDataBase();
        }

        #endregion

        #region Events

        private void btnUpdateBalances_Click(object sender, RoutedEventArgs e)
        {
            updateBalances();

            if (actualPrincipalContent is PartialTransactions)
            {
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
                loadAccounts();
            }
        }

        private async void btnUpdatePrices_Click(object sender, RoutedEventArgs e)
        {
            await updatePrices();

            if (actualPrincipalContent is PartialPortfolio)
            {
                ((PartialPortfolio)actualPrincipalContent).loadPortfolio();
                loadAccounts();
            }
        }


        private async void btnUpdateReportWeb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                GoogleSheetsUpdater googleSheetsUpdater = new();
                await googleSheetsUpdater.UpdateSheet();
                Mouse.OverrideCursor = null;
                MessageBox.Show("Proceso completado", "Actualizar ReportWeb");
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;
                MessageBox.Show("Error mientras actualizaba ReportWeb: " + ex.Message, "Actualizar ReportWeb");
            }
        }

        private void btnReminders_Click(object sender, RoutedEventArgs e)
        {
            toggleViews(eViews.Reminders);
        }

        private void btnPortfolio_Click(object sender, RoutedEventArgs e)
        {
            toggleViews(eViews.Portfolio);
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

        private async void frmInicio_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
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
                        await Task.Run(() => DependencyConfig.iExpirationsRemindersService.generateAutoregister());
                        loadAccounts();
                    }
                    else if (actualPrincipalContent is PartialPortfolio)
                    {
                        ((PartialPortfolio)actualPrincipalContent).loadPortfolio();
                        loadAccounts();
                    }
                    break;
            }
        }

        private async void frmInicio_Loaded(object sender, RoutedEventArgs e)
        {
            loadCalendar();
            await Task.Run(() => DependencyConfig.iExpirationsRemindersService.generateAutoregister());
            loadAccounts();
            toggleViews(eViews.Home);
        }

        private void lvAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvAccounts.SelectedValue != null)
            {
                toggleViews(eViews.Transactions);

                if (actualPrincipalContent is PartialReminders)
                {
                    ((PartialReminders)actualPrincipalContent).loadReminders();
                }

                if (actualPrincipalContent is PartialTransactions)
                {
                    ((PartialTransactions)actualPrincipalContent).setColumnVisibility((AccountsView?)lvAccounts.SelectedValue);
                }
            }
        }

        private void btnAllAccounts_Click(object sender, RoutedEventArgs e)
        {
            lvAccounts.SelectedItem = null;

            toggleViews(eViews.Transactions);

            if (actualPrincipalContent is PartialTransactions)
            {
                ((PartialTransactions)actualPrincipalContent).setColumnVisibility();
            }
        }

        private void lvAccounts_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            autoResizeListView();
        }

        private void btnMntAccounts_Click(object sender, RoutedEventArgs e)
        {
            FrmAccountsList frm = new();
            frm.ShowDialog();
            loadAccounts();

            if (actualPrincipalContent is PartialTransactions)
            {
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
            }
            else if (actualPrincipalContent is PartialHome)
            {
                ((PartialHome)actualPrincipalContent).loadCharts();
            }
        }

        private void btnMntPersons_Click(object sender, RoutedEventArgs e)
        {
            FrmPersonsList frm = new();
            frm.ShowDialog();

            if (actualPrincipalContent is PartialTransactions)
            {
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
            }
        }

        private void btnMntInvestmentProducts_Click(object sender, RoutedEventArgs e)
        {
            FrmInvestmentProductsList frm = new();
            frm.ShowDialog();

            if (actualPrincipalContent is PartialTransactions)
            {
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
            }
            else if (actualPrincipalContent is PartialPortfolio)
            {
                ((PartialPortfolio)actualPrincipalContent).loadPortfolio();
            }
        }

        private void btnMntCategories_Click(object sender, RoutedEventArgs e)
        {
            FrmCategoriesList frm = new();
            frm.ShowDialog();

            if (actualPrincipalContent is PartialTransactions)
            {
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
            }
            else if (actualPrincipalContent is PartialHome)
            {
                ((PartialHome)actualPrincipalContent).loadCharts();
            }
        }

        private void btnMntTags_Click(object sender, RoutedEventArgs e)
        {
            FrmTagsList frm = new();
            frm.ShowDialog();

            if (actualPrincipalContent is PartialTransactions)
            {
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
            }
        }

        private async void btnMntReminders_Click(object sender, RoutedEventArgs e)
        {
            FrmTransactionReminderList frm = new();
            frm.ShowDialog();

            if (actualPrincipalContent is PartialReminders)
            {
                ((PartialReminders)actualPrincipalContent).loadReminders();
            }

            await Task.Run(() => DependencyConfig.iExpirationsRemindersService.generateAutoregister());
            loadAccounts();
        }


        #endregion

        #region Functions

        public async void refreshBalance()
        {
            try
            {
                foreach (AccountsView accounts in lvAccounts.ItemsSource)
                {
                    accounts.balance = await Task.Run(() => DependencyConfig.iAccountsService.getBalanceByAccount(accounts.id));
                }

                viewAccounts.Refresh();

                autoResizeListView();
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine((DateTime.Now.ToString() + ": Error - " + ex.Message));
            }
        }


        private void openNewTransaction()
        {
            FrmTransaction frm = lvAccounts.SelectedItem == null ? new FrmTransaction() : new FrmTransaction(((AccountsView)lvAccounts.SelectedItem).id);
            frm.ShowDialog();
            loadAccounts();

            if (actualPrincipalContent is PartialTransactions)
            {
                ((PartialTransactions)actualPrincipalContent).loadTransactions();
            }
        }

        private void loadCalendar()
        {
            DependencyConfig.iDateCalendarService.fillCalendar();
        }

        private void toggleViews(eViews views)
        {
            Page? win = null;
            principalContent.Content = null;

            switch (views)
            {
                case eViews.Home:
                    lvAccounts.SelectedIndex = -1;
                    win = new PartialHome(this);
                    break;
                case eViews.Transactions:
                    win = new PartialTransactions(this);
                    break;
                case eViews.Reminders:
                    lvAccounts.SelectedIndex = -1;
                    win = new PartialReminders(this);
                    break;
                case eViews.Portfolio:
                    lvAccounts.SelectedIndex = -1;
                    win = new PartialPortfolio(this);
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
            GridView? gv = lvAccounts.View as GridView;

            if (remainingSpace > 0)
            {
                gv.Columns[0].Width = Math.Ceiling(remainingSpace * .6);
                gv.Columns[1].Width = Math.Ceiling(remainingSpace * .4);
            }
        }

        public void loadAccounts()
        {
            AccountsView? accountsView = null;
            if (lvAccounts != null && lvAccounts.SelectedItem != null)
            {
                accountsView = lvAccounts.SelectedValue as AccountsView;
            }

            List<AccountsView>? accountsViews = DependencyConfig.iAccountsService.getAllOpenedListView();
            viewAccounts = CollectionViewSource.GetDefaultView(accountsViews);
            lvAccounts.ItemsSource = viewAccounts;
            viewAccounts.GroupDescriptions.Add(new PropertyGroupDescription("accountsTypesdescription"));
            viewAccounts.SortDescriptions.Add(new SortDescription("accountsTypesid", ListSortDirection.Ascending));
            refreshBalance();

            if (accountsView != null)
            {
                int index = -1;
                for (int i = 0; i < lvAccounts.Items.Count - 1; i++)
                {
                    if (((AccountsView)lvAccounts.Items[i]).id.Equals(accountsView.id))
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    lvAccounts.SelectedIndex = index;
                }
                else
                {
                    toggleViews(eViews.Home);
                }
            }

        }

        private async void updateBalances()
        {
            try
            {
                HashSet<Accounts?>? laccounts = DependencyConfig.iAccountsService.getAll();

                if (laccounts != null)
                {
                    LoadDialog loadDialog = new(laccounts.Count);
                    loadDialog.Show();

                    foreach (Accounts? accounts in laccounts)
                    {
                        Transactions? tFirst = DependencyConfig.iTransactionsService.getByAccount(accounts)?.FirstOrDefault();
                        if (tFirst != null)
                        {
                            await Task.Run(() => DependencyConfig.iTransactionsService.refreshBalanceTransactions(tFirst, true));
                        }
                        loadDialog.performeStep();
                    }

                    loadDialog.Close();
                    MessageBox.Show("Actualizado con exito!", "Actualización de saldos");
                }
                else
                {
                    MessageBox.Show("No hay productos financieros a actualizar", "Actualización de saldos");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha un ocurrido un error: " + ex.Message, "Actualización de saldos");
            }
        }

        private async Task updatePrices()
        {
            try
            {
                List<InvestmentProducts?>? lInvestmentProducts = DependencyConfig.iInvestmentProductsService.getAll()?.Where(x => !String.IsNullOrWhiteSpace(x.url) || x.active == true).ToList();

                if (lInvestmentProducts != null)
                {
                    LoadDialog loadDialog = new(lInvestmentProducts.Count);
                    loadDialog.Show();

                    foreach (InvestmentProducts? investmentProducts in lInvestmentProducts)
                    {
                        await DependencyConfig.iInvestmentProductsPricesService.getPricesOnlineAsync(investmentProducts);
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
