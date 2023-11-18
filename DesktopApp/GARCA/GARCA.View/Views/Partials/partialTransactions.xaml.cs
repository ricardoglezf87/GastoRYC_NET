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

        #endregion

        #region Constructor

        public PartialTransactions(MainWindow parentForm)
        {
            InitializeComponent();
            this.parentForm = parentForm;
        }

        #endregion

        #region Events

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTransactions();
        }

        private void gvTransactions_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer eliminar este movimiento?", "Eliminación movimiento", MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private async void ButtonSplit_Click(object sender, RoutedEventArgs e)
        {
            var transactions = (Transactions)gvTransactions.SelectedItem;
            FrmSplitsList frm = new(transactions);
            frm.ShowDialog();
            await iTransactionsService.UpdateTransactionAfterSplits(transactions);
            await iTransactionsService.RefreshBalanceAllTransactions();
            LoadTransactions();
            await parentForm.LoadAccounts();
        }

        private async void gvTransactions_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Transactions transactions in e.Items)
            {
                await RemoveTransaction(transactions);
            }

            LoadTransactions();
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
                        transactionsReminders.PeriodsRemindersid = (int)PeriodsRemindersService.EPeriodsReminders.Monthly;
                        transactionsReminders.Accountid = transactions.AccountsId;
                        transactionsReminders.Personid = transactions.PersonsId;
                        transactionsReminders.Categoryid = transactions.CategoriesId;
                        transactionsReminders.Memo = transactions.Memo;
                        transactionsReminders.AmountIn = transactions.AmountIn;
                        transactionsReminders.AmountOut = transactions.AmountOut;
                        transactionsReminders.Tagid = transactions.TagsId;
                        transactionsReminders.TransactionStatusid = (int)TransactionsStatusService.ETransactionsTypes.Pending;

                        transactionsReminders = await iTransactionsRemindersService.Save(transactionsReminders);

                        foreach (var splits in await iSplitsService.GetbyTransactionid(transactions.Id))
                        {
                            SplitsReminders splitsReminders = new();
                            splitsReminders.Transactionid = transactionsReminders.Id;
                            splitsReminders.Categoryid = splits.Categoryid;
                            splitsReminders.Memo = splits.Memo;
                            splitsReminders.AmountIn = splits.AmountIn;
                            splitsReminders.AmountOut = splits.AmountOut;
                            splitsReminders.Tagid = splits.Tagid;
                            await iSplitsRemindersService.Update(splitsReminders);
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
                LoadTransactions();
                await parentForm.LoadAccounts();
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (gvTransactions.SelectedItems != null && gvTransactions.SelectedItems.Count > 0)
            {
                foreach (Transactions transactions in gvTransactions.SelectedItems)
                {
                    await RemoveTransaction(transactions);
                }
                LoadTransactions();
                await parentForm.LoadAccounts();
            }
            else
            {
                MessageBox.Show("Tiene que seleccionar alguna línea.", "Cambio estado movimieno");
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
                    await iTransactionsService.Update(transactions);
                }
                await parentForm.LoadAccounts();
                LoadTransactions();
            } 
            else
            {
                MessageBox.Show("Tiene que seleccionar alguna línea.", "Cambio estado movimieno");
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
                    await iTransactionsService.Update(transactions);
                }
                await parentForm.LoadAccounts();
                LoadTransactions();
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
                    await iTransactionsService.Update(transactions);
                }
                await parentForm.LoadAccounts();
                LoadTransactions();
            }
            else
            {
                MessageBox.Show("Tiene que seleccionar alguna línea.", "Cambio estado movimieno");
            }
        }

        #endregion

        #region Functions        

        public void LoadTransactions()
        {
            SetColumnVisibility(TransactionViewModel.AccountsSelected);
        }

        public void SetColumnVisibility(Accounts? accountSelected = null)
        {
            TransactionViewModel.AccountsSelected = accountSelected;

            if (gvTransactions.View != null)
            {
                if (accountSelected != null)
                {
                    gvTransactions.Columns["Account.Description"].IsHidden = true;

                    if (TransactionViewModel.AccountsSelected.AccountsTypesId == (int)AccountsTypesService.EAccountsTypes.Invests)
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
                    gvTransactions.Columns["Account.Description"].IsHidden = false;
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
                if (transactions.Splits != null)
                {
                    var lSplits = transactions.Splits.ToList();
                    for (var i = 0; i < lSplits.Count; i++)
                    {
                        var splits = lSplits[i];
                        if (splits.Tranferid != null)
                        {
                            await iTransactionsService.Delete(await iTransactionsService.GetById(splits.Tranferid ?? -99));
                        }

                        await iSplitsService.Delete(splits);
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
