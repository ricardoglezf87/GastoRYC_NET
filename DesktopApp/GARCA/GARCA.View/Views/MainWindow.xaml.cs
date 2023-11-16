using GARCA.Models;
using GARCA.View.ViewModels;
using GARCA.View.Views;
using GARCA.View.Views.Common;
using GARCA.WebReport;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA
{
    public partial class MainWindow : RibbonWindow
    {

        #region Variables

        private ICollectionView? viewAccounts;
        private Page? actualPrincipalContent;

        private enum EViews
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

            iRycContextService.MakeBackup();
            iRycContextService.MigrateDataBase();
        }

        #endregion

        #region Events

        private async void btnArchiveTransactions_Click(object sender, RoutedEventArgs e)
        {
            String strDate = Microsoft.VisualBasic.Interaction.InputBox("Inserte la fecha(dd/mm/yyyy) a archivar:", "Archivar transacciones");
            if (strDate == null || !strDate.Contains("/") || strDate.Split("/").Count() != 3)
            {
                MessageBox.Show("Debe colocar una fecha valida", "Archivar transacciones");
            }
            else
            {
                DateTime date;
                date = DateTime.ParseExact(strDate,"dd/MM/yy", CultureInfo.InvariantCulture);

                Mouse.OverrideCursor = Cursors.Wait;

                iRycContextService.MakeBackup();

                await iTransactionsArchivedService.ArchiveTransactions(date);

                if (actualPrincipalContent is PartialTransactions transactions)
                {
                    transactions.LoadTransactions();
                    await LoadAccounts();
                }

                Mouse.OverrideCursor = null;

                MessageBox.Show("Transacciones archivadas!", "Archivar transacciones");
            }
        }

        private async void btnUpdateBalances_Click(object sender, RoutedEventArgs e)
        {
            await UpdateBalances();

            if (actualPrincipalContent is PartialTransactions transactions)
            {
                transactions.LoadTransactions();
                await LoadAccounts();
            }
        }

        private async void btnUpdatePrices_Click(object sender, RoutedEventArgs e)
        {
            await UpdatePrices();

            if (actualPrincipalContent is PartialPortfolio portfolio)
            {
                await portfolio.LoadPortfolio();
                await LoadAccounts();
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
            ToggleViews(EViews.Reminders);
        }

        private void btnPortfolio_Click(object sender, RoutedEventArgs e)
        {
            ToggleViews(EViews.Portfolio);
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            ToggleViews(EViews.Home);
        }

        private async void btnNewTransaction_Click(object sender, RoutedEventArgs e)
        {
            await OpenNewTransaction();
        }

        private async void btnAddTransaction_Click(object sender, RoutedEventArgs e)
        {
            await OpenNewTransaction();
        }

        private async void frmInicio_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    await OpenNewTransaction();
                    break;
                case Key.F4:
                    var maxItem = Microsoft.VisualBasic.Interaction.InputBox("Inserte un numero elementos a cargar:", "Transacción");
                    if (!String.IsNullOrWhiteSpace(maxItem))
                    {
                        TransactionViewModel.MaxItem = Int32.Parse(maxItem);
                        if (actualPrincipalContent is PartialTransactions transactions)
                        {
                            await LoadAccounts();
                            transactions.LoadTransactions();
                        }
                    }
                    break;
                case Key.F5:
                    switch (actualPrincipalContent)
                    {
                        case PartialHome home:
                            await home.LoadCharts();
                            break;
                        case PartialTransactions transactions:
                            await LoadAccounts();
                            transactions.LoadTransactions();
                            break;
                        case PartialReminders reminders:
                            await reminders.LoadReminders();
                            await iExpirationsRemindersService.GenerateAutoregister();
                            await LoadAccounts();
                            break;
                        case PartialPortfolio portfolio:
                            await portfolio.LoadPortfolio();
                            await LoadAccounts();
                            break;
                    }
                    break;
            }
        }

        private async void frmInicio_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCalendar();
            await iExpirationsRemindersService.GenerateAutoregister();
            await LoadAccounts();
            ToggleViews(EViews.Home);
        }

        private async void lvAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvAccounts.SelectedValue != null)
            {
                ToggleViews(EViews.Transactions);

                switch (actualPrincipalContent)
                {
                    case PartialReminders reminders:
                        await reminders.LoadReminders();
                        break;
                    case PartialTransactions transactions:
                        transactions.SetColumnVisibility((Accounts)lvAccounts.SelectedValue);
                        break;
                }
            }
        }

        private void btnAllAccounts_Click(object sender, RoutedEventArgs e)
        {
            lvAccounts.SelectedItem = null;

            ToggleViews(EViews.Transactions);

            if (actualPrincipalContent is PartialTransactions transactions)
            {
                transactions.SetColumnVisibility();
            }
        }

        private void lvAccounts_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AutoResizeListView();
        }

        private async void btnMntAccounts_Click(object sender, RoutedEventArgs e)
        {
            FrmAccountsList frm = new();
            frm.ShowDialog();
            await LoadAccounts();

            switch (actualPrincipalContent)
            {
                case PartialTransactions transactions:
                    transactions.LoadTransactions();
                    break;
                case PartialHome home:
                    await home.LoadCharts();
                    break;
            }
        }

        private void btnMntPersons_Click(object sender, RoutedEventArgs e)
        {
            FrmPersonsList frm = new();
            frm.ShowDialog();

            if (actualPrincipalContent is PartialTransactions transactions)
            {
                transactions.LoadTransactions();
            }
        }

        private async void btnMntInvestmentProducts_Click(object sender, RoutedEventArgs e)
        {
            FrmInvestmentProductsList frm = new();
            frm.ShowDialog();

            switch (actualPrincipalContent)
            {
                case PartialTransactions transactions:
                    transactions.LoadTransactions();
                    break;
                case PartialPortfolio portfolio:
                    await portfolio.LoadPortfolio();
                    break;
            }
        }

        private async void btnMntCategories_Click(object sender, RoutedEventArgs e)
        {
            FrmCategoriesList frm = new();
            frm.ShowDialog();

            switch (actualPrincipalContent)
            {
                case PartialTransactions transactions:
                    transactions.LoadTransactions();
                    break;
                case PartialHome home:
                    await home.LoadCharts();
                    break;
            }
        }

        private void btnMntTags_Click(object sender, RoutedEventArgs e)
        {
            FrmTagsList frm = new();
            frm.ShowDialog();

            if (actualPrincipalContent is PartialTransactions transactions)
            {
                transactions.LoadTransactions();
            }
        }

        private async void btnMntReminders_Click(object sender, RoutedEventArgs e)
        {
            FrmTransactionReminderList frm = new();
            frm.ShowDialog();

            if (actualPrincipalContent is PartialReminders reminders)
            {
                await reminders.LoadReminders();
            }

            await iExpirationsRemindersService.GenerateAutoregister();
            await LoadAccounts();
        }


        #endregion

        #region Functions

        private async Task RefreshBalance()
        {
            try
            {
                foreach (Accounts accounts in lvAccounts.ItemsSource)
                {
                    accounts.Balance = await iAccountsService.GetBalanceByAccount(accounts.Id);
                }

                viewAccounts.Refresh();

                AutoResizeListView();
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(DateTime.Now + ": Error - " + ex.Message);
            }
        }


        private async Task OpenNewTransaction()
        {
            var frm = lvAccounts.SelectedItem == null ? new FrmTransaction() : new FrmTransaction(((Accounts)lvAccounts.SelectedItem).Id);
            frm.ShowDialog();
            await LoadAccounts();

            if (actualPrincipalContent is PartialTransactions transactions)
            {
                transactions.LoadTransactions();
            }
        }

        private void LoadCalendar()
        {
            iDateCalendarService.FillCalendar();
        }

        private void ToggleViews(EViews views)
        {
            Page? win = null;
            principalContent.Content = null;

            switch (views)
            {
                case EViews.Home:
                    lvAccounts.SelectedIndex = -1;
                    win = new PartialHome();
                    break;
                case EViews.Transactions:
                    win = new PartialTransactions(this);
                    break;
                case EViews.Reminders:
                    lvAccounts.SelectedIndex = -1;
                    win = new PartialReminders(this);
                    break;
                case EViews.Portfolio:
                    lvAccounts.SelectedIndex = -1;
                    win = new PartialPortfolio();
                    break;
            }

            if (win != null)
            {
                actualPrincipalContent = win;
                principalContent.Content = win;
            }
        }

        private void AutoResizeListView()
        {
            var remainingSpace = lvAccounts.ActualWidth * .93;
            var gv = lvAccounts.View as GridView;

            if (remainingSpace > 0)
            {
                gv.Columns[0].Width = Math.Ceiling(remainingSpace * .6);
                gv.Columns[1].Width = Math.Ceiling(remainingSpace * .4);
            }
        }

        public async Task LoadAccounts()
        {
            Accounts? account = null;
            if (lvAccounts != null && lvAccounts.SelectedItem != null)
            {
                account = lvAccounts.SelectedValue as Accounts;
            }
            
            if (lvAccounts != null)
            {

                var accounts = iAccountsService.GetAllOpened();
                viewAccounts = CollectionViewSource.GetDefaultView(accounts);
                lvAccounts.ItemsSource = viewAccounts;
                viewAccounts.GroupDescriptions.Add(new PropertyGroupDescription("AccountsTypesdescription"));
                viewAccounts.SortDescriptions.Add(new SortDescription("AccountsTypesid", ListSortDirection.Ascending));
                await RefreshBalance();

                if (account != null)
                {
                    var index = -1;
                    for (var i = 0; i < lvAccounts.Items.Count - 1; i++)
                    {
                        if (((Accounts)lvAccounts.Items[i]).Id.Equals(account.Id))
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
                        ToggleViews(EViews.Home);
                    }
                }
            }

        }

        private async Task UpdateBalances()
        {
            try
            {
                var laccounts = await iAccountsService.GetAll();

                if (laccounts != null)
                {
                    LoadDialog loadDialog = new(laccounts.Count());
                    loadDialog.Show();

                    foreach (var accounts in laccounts)
                    {
                        await iTransactionsService.RefreshBalanceTransactions(accounts);
                        loadDialog.PerformeStep();
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

        private async Task UpdatePrices()
        {
            try
            {
                var lInvestmentProducts = (await iInvestmentProductsService.GetAll())?.Where(x => !String.IsNullOrWhiteSpace(x.Url) || x.Active == true);

                if (lInvestmentProducts != null)
                {
                    LoadDialog loadDialog = new(lInvestmentProducts.Count());
                    loadDialog.Show();

                    foreach (var investmentProducts in lInvestmentProducts)
                    {
                        await iInvestmentProductsPricesService.getPricesOnlineAsync(investmentProducts);
                        loadDialog.PerformeStep();
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
