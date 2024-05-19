using GARCA.Data.Services;
using GARCA.Models;
using GARCA.Utils.Extensions;
using GARCA.View.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.View.Views
{
    /// <summary>
    /// Lógica de interacción para partialTransactions.xaml
    /// </summary>
    public partial class PartialTransactions : Page
    {

        #region Variables

        private readonly MainWindow parentForm;
        public Accounts? AccountSelected { get; set; }
        public TransactionViewModel TransactionsData { get; set; }

        #endregion

        #region Constructor

        public PartialTransactions(MainWindow parentForm)
        {
            TransactionsData = new TransactionViewModel();
            InitializeComponent();
            this.parentForm = parentForm;
        }

        #endregion

        #region Events

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            gvTransactions.ItemsSource = await TransactionsData.GetSource();
            LoadTransactions();
        }

        private void gvTransactions_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer eliminar este movimiento?", "Eliminación movimientos", MessageBoxButton.YesNo,
                MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private async void ButtonSplit_Click(object sender, RoutedEventArgs e)
        {
            var transactions = (Transactions)gvTransactions.SelectedItem;
            FrmSplitsList frm = new(transactions);
            frm.ShowDialog();          
            await RefreshData();
            await parentForm.LoadAccounts();
        }



        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Implementar funcionalidad
            MessageBox.Show("Funcionalidad no implementada");
        }

        private async void btnAddReminder_Click(object sender, RoutedEventArgs e)
        {
            if (gvTransactions.SelectedItems != null && gvTransactions.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Se va a proceder a crear los recordatorios", "Crear Recordatorio", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (Transactions transactions in gvTransactions.SelectedItems)
                    {
                        TransactionsReminders? transactionsReminders = new();
                        transactionsReminders.Date = transactions.Date;
                        transactionsReminders.PeriodsRemindersId = (int)PeriodsRemindersService.EPeriodsReminders.Monthly;
                        transactionsReminders.AccountsId = transactions.AccountsId;
                        transactionsReminders.PersonsId = transactions.PersonsId;
                        transactionsReminders.CategoriesId = transactions.CategoriesId;
                        transactionsReminders.Memo = transactions.Memo;
                        transactionsReminders.AmountIn = transactions.AmountIn;
                        transactionsReminders.AmountOut = transactions.AmountOut;
                        transactionsReminders.TagsId = transactions.TagsId;
                        transactionsReminders.TransactionsStatusId = (int)TransactionsStatusService.ETransactionsTypes.Pending;

                        transactionsReminders = await iTransactionsRemindersService.Save(transactionsReminders);

                        foreach (var splits in await iSplitsService.GetbyTransactionid(transactions.Id))
                        {
                            SplitsReminders splitsReminders = new();
                            splitsReminders.TransactionsId = transactionsReminders.Id;
                            splitsReminders.CategoriesId = splits.CategoriesId;
                            splitsReminders.Memo = splits.Memo;
                            splitsReminders.AmountIn = splits.AmountIn;
                            splitsReminders.AmountOut = splits.AmountOut;
                            splitsReminders.TagsId = splits.TagsId;
                            await iSplitsRemindersService.Save(splitsReminders);
                        }

                        FrmTransactionReminders frm = new(transactionsReminders);
                        frm.ShowDialog();
                        if (frm.WindowsResult == WindowsExtension.EWindowsResult.Sucess)
                        {
                            MessageBox.Show("Recordatorio creado.", "Crear Recordatorio");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No se ha creado el recordatorios.", "Crear Recordatorio");
                }
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

        private async void gvTransactions_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (gvTransactions.CurrentItem != null)
            {
                FrmTransaction frm = new((Transactions)gvTransactions.CurrentItem);
                frm.ShowDialog();
                await parentForm.RefreshBalance();
            }
        }

        public async Task RefreshData()
        {
            await TransactionsData.LoadData();
            gvTransactions.ItemsSource = await TransactionsData.GetSource();
            LoadTransactions();
        }

        private async void gvTransactions_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            if (e.Items != null && e.Items.Count > 0)
            {
                foreach (Transactions transactions in e.Items)
                {
                    await RemoveTransaction(transactions);
                }

                await parentForm.RefreshBalance();
            }
            else
            {
                MessageBox.Show("Tiene que seleccionar alguna línea.", "Cambio estado movimiento");
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (gvTransactions.SelectedItems != null && gvTransactions.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Esta seguro de querer eliminar este movimiento?", "Eliminación movimientos", MessageBoxButton.YesNo,
                    MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    return;
                }

                foreach (Transactions transactions in gvTransactions.SelectedItems)
                {
                    await RemoveTransaction(transactions);
                }

                await parentForm.RefreshBalance();
            }
            else
            {
                MessageBox.Show("Tiene que seleccionar alguna línea.", "Cambio estado movimiento");
            }
        }

        private async void btnPending_Click(object sender, RoutedEventArgs e)
        {
            if (gvTransactions.SelectedItems != null && gvTransactions.SelectedItems.Count > 0)
            {
                foreach (Transactions transactions in gvTransactions.SelectedItems)
                {
                    transactions.TransactionsStatusId = (int)TransactionsStatusService.ETransactionsTypes.Pending;
                    transactions.TransactionsStatus = await iTransactionsStatusService.GetById(transactions.TransactionsStatusId ?? -99);
                    await iTransactionsService.Save(transactions);
                }
                await RefreshData();
            }
            else
            {
                MessageBox.Show("Tiene que seleccionar alguna línea.", "Cambio estado movimiento");
            }
        }

        private async void btnProvisional_Click(object sender, RoutedEventArgs e)
        {
            if (gvTransactions.SelectedItems != null && gvTransactions.SelectedItems.Count > 0)
            {
                foreach (Transactions transactions in gvTransactions.SelectedItems)
                {
                    transactions.TransactionsStatusId = (int)TransactionsStatusService.ETransactionsTypes.Provisional;
                    transactions.TransactionsStatus = await iTransactionsStatusService.GetById(transactions.TransactionsStatusId ?? -99);
                    await iTransactionsService.Save(transactions);
                }
                await RefreshData();
            }
            else
            {
                MessageBox.Show("Tiene que seleccionar alguna línea.", "Cambio estado movimieno");
            }
        }

        private async void btnReconciled_Click(object sender, RoutedEventArgs e)
        {
            if (gvTransactions.SelectedItems != null && gvTransactions.SelectedItems.Count > 0)
            {
                foreach (Transactions transactions in gvTransactions.SelectedItems)
                {
                    transactions.TransactionsStatusId = (int)TransactionsStatusService.ETransactionsTypes.Reconciled;
                    transactions.TransactionsStatus = await iTransactionsStatusService.GetById(transactions.TransactionsStatusId ?? -99);
                    await iTransactionsService.Save(transactions);
                }
                await RefreshData();
            }
            else
            {
                MessageBox.Show("Tiene que seleccionar alguna línea.", "Cambio estado movimieno");
            }
        }

        #endregion

        #region Functions   

        public bool FilterRecords(object o)
        {
            if (AccountSelected == null)
            {
                return true;
            }

            var item = o as Transactions;

            return item != null && item.AccountsId.Equals(AccountSelected.Id);
        }

        public void LoadTransactions()
        {
            if (gvTransactions.View != null)
            {
                gvTransactions.View.Filter = FilterRecords;
                gvTransactions.View.RefreshFilter();
            }
            SetColumnVisibility();
        }

        public void SetColumnVisibility()
        {
            if (gvTransactions.View != null)
            {
                if (AccountSelected != null)
                {
                    gvTransactions.Columns["Accounts.Description"].IsHidden = true;

                    if (AccountSelected.AccountsTypesId == (int)AccountsTypesService.EAccountsTypes.Invests)
                    {
                        gvTransactions.Columns["NumShares"].IsHidden = false;
                        gvTransactions.Columns["PricesShares"].IsHidden = false;
                        gvTransactions.Columns["AmountIn"].IsHidden = true;
                        gvTransactions.Columns["AmountOut"].IsHidden = true;
                    }
                    else
                    {
                        gvTransactions.Columns["NumShares"].IsHidden = true;
                        gvTransactions.Columns["PricesShares"].IsHidden = true;
                        gvTransactions.Columns["AmountIn"].IsHidden = false;
                        gvTransactions.Columns["AmountOut"].IsHidden = false;
                    }
                }
                else
                {
                    gvTransactions.Columns["Accounts.Description"].IsHidden = false;
                }
            }
        }

        private async Task RemoveTransaction(Transactions transactions)
        {
            if (transactions.TranferSplitId != null)
            {
                MessageBox.Show("El movimiento Id: " + transactions.Id +
                    " de fecha: " + transactions.Date.ToShortDateString() + " viene de una transferencia desde split, para borrar diríjase al split que lo generó.", "Eliminación movimiento");
            }
            else
            {
                var splits = await iSplitsService.GetbyTransactionid(transactions.Id);
                if (splits != null)
                {
                    var lSplits = splits.ToList();
                    for (var i = 0; i < lSplits.Count; i++)
                    {
                        var split = lSplits[i];
                        if (split.TranferId != null)
                        {
                            await iTransactionsService.Delete(await iTransactionsService.GetById(split.TranferId ?? -99));
                        }

                        await iSplitsService.Delete(split);
                    }
                }

                if (transactions.TranferId != null)
                {
                    await iTransactionsService.Delete(await iTransactionsService.GetById(transactions.TranferId ?? -99));
                }

                await iTransactionsService.Delete(transactions);
            }
        }


        #endregion       
    }
}
