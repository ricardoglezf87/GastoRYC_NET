using GARCA.Utils.IOC;
using GARCA.View.ViewModels;
using GARCA.View.Views;
using GARCA.View.Views.Common;
using GARCA.WebReport;
using Syncfusion.SfSkinManager;
using Syncfusion.Windows.Tools.Controls;
using System;
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

            DependencyConfigView.RycContextServiceView.MakeBackup();
            DependencyConfigView.RycContextServiceView.MigrateDataBase();
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
                date = DateTime.Parse(strDate);

                Mouse.OverrideCursor = Cursors.Wait;

                DependencyConfigView.RycContextServiceView.MakeBackup();

                await DependencyConfigView.TransactionsArchivedServiceView.ArchiveTransactions(date);

                if (actualPrincipalContent is PartialTransactions transactions)
                {
                    transactions.LoadTransactions();
                    LoadAccounts();
                }

                Mouse.OverrideCursor = null;

                MessageBox.Show("Transacciones archivadas!", "Archivar transacciones");
            }
        }

        private void btnUpdateBalances_Click(object sender, RoutedEventArgs e)
        {
            UpdateBalances();

            if (actualPrincipalContent is PartialTransactions transactions)
            {
                transactions.LoadTransactions();
                LoadAccounts();
            }
        }

        private async void btnUpdatePrices_Click(object sender, RoutedEventArgs e)
        {
            await UpdatePrices();

            if (actualPrincipalContent is PartialPortfolio portfolio)
            {
                portfolio.LoadPortfolio();
                LoadAccounts();
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

        private void btnNewTransaction_Click(object sender, RoutedEventArgs e)
        {
            OpenNewTransaction();
        }

        private void btnAddTransaction_Click(object sender, RoutedEventArgs e)
        {
            OpenNewTransaction();
        }

        private async void frmInicio_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    OpenNewTransaction();
                    break;
                case Key.F4:
                    var maxItem = Microsoft.VisualBasic.Interaction.InputBox("Inserte un numero elementos a cargar:", "Transacción");
                    if (!String.IsNullOrWhiteSpace(maxItem))
                    {
                        TransactionViewModel.maxItem = Int32.Parse(maxItem);
                        if (actualPrincipalContent is PartialTransactions transactions)
                        {
                            LoadAccounts();
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
                            LoadAccounts();
                            transactions.LoadTransactions();
                            break;
                        case PartialReminders reminders:
                            reminders.LoadReminders();
                            await Task.Run(() => DependencyConfigView.ExpirationsRemindersServiceView.GenerateAutoregister());
                            LoadAccounts();
                            break;
                        case PartialPortfolio portfolio:
                            portfolio.LoadPortfolio();
                            LoadAccounts();
                            break;
                    }
                    break;
            }
        }

        private async void frmInicio_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCalendar();
            await Task.Run(() => DependencyConfigView.ExpirationsRemindersServiceView.GenerateAutoregister());
            LoadAccounts();
            ToggleViews(EViews.Home);
        }

        private void lvAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvAccounts.SelectedValue != null)
            {
                ToggleViews(EViews.Transactions);

                switch (actualPrincipalContent)
                {
                    case PartialReminders reminders:
                        reminders.LoadReminders();
                        break;
                    case PartialTransactions transactions:
                        transactions.SetColumnVisibility((AccountsView?)lvAccounts.SelectedValue);
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
            LoadAccounts();

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

        private void btnMntInvestmentProducts_Click(object sender, RoutedEventArgs e)
        {
            FrmInvestmentProductsList frm = new();
            frm.ShowDialog();

            switch (actualPrincipalContent)
            {
                case PartialTransactions transactions:
                    transactions.LoadTransactions();
                    break;
                case PartialPortfolio portfolio:
                    portfolio.LoadPortfolio();
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
                reminders.LoadReminders();
            }

            await Task.Run(() => DependencyConfigView.ExpirationsRemindersServiceView.GenerateAutoregister());
            LoadAccounts();
        }


        #endregion

        #region Functions

        private async void RefreshBalance()
        {
            try
            {
                foreach (AccountsView accounts in lvAccounts.ItemsSource)
                {
                    accounts.Balance = await Task.Run(() => DependencyConfigView.AccountsServiceView.GetBalanceByAccount(accounts.Id));
                }

                viewAccounts.Refresh();

                AutoResizeListView();
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(DateTime.Now + ": Error - " + ex.Message);
            }
        }


        private void OpenNewTransaction()
        {
            var frm = lvAccounts.SelectedItem == null ? new FrmTransaction() : new FrmTransaction(((AccountsView)lvAccounts.SelectedItem).Id);
            frm.ShowDialog();
            LoadAccounts();

            if (actualPrincipalContent is PartialTransactions transactions)
            {
                transactions.LoadTransactions();
            }
        }

        private void LoadCalendar()
        {
            DependencyConfigView.DateCalendarServiceView.FillCalendar();
        }

        private void ToggleViews(EViews views)
        {
            Page? win = null;
            principalContent.Content = null;

            switch (views)
            {
                case EViews.Home:
                    lvAccounts.SelectedIndex = -1;
                    win = new PartialHome(this);
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
                    win = new PartialPortfolio(this);
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

        public void LoadAccounts()
        {
            AccountsView? accountsView = null;
            if (lvAccounts != null && lvAccounts.SelectedItem != null)
            {
                accountsView = lvAccounts.SelectedValue as AccountsView;
            }

            var accountsViews = DependencyConfigView.AccountsServiceView.GetAllOpenedListView();
            viewAccounts = CollectionViewSource.GetDefaultView(accountsViews);
            lvAccounts.ItemsSource = viewAccounts;
            viewAccounts.GroupDescriptions.Add(new PropertyGroupDescription("AccountsTypesdescription"));
            viewAccounts.SortDescriptions.Add(new SortDescription("AccountsTypesid", ListSortDirection.Ascending));
            RefreshBalance();

            if (accountsView != null)
            {
                var index = -1;
                for (var i = 0; i < lvAccounts.Items.Count - 1; i++)
                {
                    if (((AccountsView)lvAccounts.Items[i]).Id.Equals(accountsView.Id))
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

        private async void UpdateBalances()
        {
            try
            {
                var laccounts = DependencyConfigView.AccountsServiceView.GetAll();

                if (laccounts != null)
                {
                    LoadDialog loadDialog = new(laccounts.Count);
                    loadDialog.Show();

                    foreach (var accounts in laccounts)
                    {
                        var tFirst = DependencyConfigView.TransactionsServiceView.GetByAccount(accounts)?.FirstOrDefault();
                        if (tFirst != null)
                        {
                            await Task.Run(() => DependencyConfigView.TransactionsServiceView.RefreshBalanceTransactions(tFirst, true, true));
                        }
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
                var lInvestmentProducts = DependencyConfigView.InvestmentProductsServiceView.GetAll()?.Where(x => !String.IsNullOrWhiteSpace(x.Url) || x.Active == true).ToList();

                if (lInvestmentProducts != null)
                {
                    LoadDialog loadDialog = new(lInvestmentProducts.Count);
                    loadDialog.Show();

                    foreach (var investmentProducts in lInvestmentProducts)
                    {
                        await DependencyConfigView.InvestmentProductsPricesServiceView.getPricesOnlineAsync(investmentProducts);
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
