using GARCA.BO.Models;
using GARCA.BO.Services;
using GARCA.Utils.IOC;
using GARCA.Utlis.Extensions;
using GARCA.View.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static GARCA.Utlis.Extensions.WindowsExtension;

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

        private void ButtonSplit_Click(object sender, RoutedEventArgs e)
        {
            var transactions = (Transactions)gvTransactions.SelectedItem;
            FrmSplitsList frm = new(transactions);
            frm.ShowDialog();
            DependencyConfig.TransactionsService.UpdateTransactionAfterSplits(transactions);
            LoadTransactions();
            parentForm.LoadAccounts();
        }

        private void gvTransactions_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Transactions transactions in e.Items)
            {
                RemoveTransaction(transactions);
            }

            LoadTransactions();
            parentForm.LoadAccounts();
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
                if (MessageBox.Show("Se va a proceder a crear los recordatorios", "Crear Recordatorio", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (Transactions transactions in gvTransactions.SelectedItems)
                    {
                        TransactionsReminders? transactionsReminders = new();
                        transactionsReminders.Date = transactions.Date;
                        transactionsReminders.PeriodsRemindersid = (int)PeriodsRemindersService.EPeriodsReminders.Monthly;
                        transactionsReminders.Accountid = transactions.Accountid;
                        transactionsReminders.Personid = transactions.Personid;
                        transactionsReminders.Categoryid = transactions.Categoryid;
                        transactionsReminders.Memo = transactions.Memo;
                        transactionsReminders.AmountIn = transactions.AmountIn;
                        transactionsReminders.AmountOut = transactions.AmountOut;
                        transactionsReminders.Tagid = transactions.Tagid;
                        transactionsReminders.TransactionStatusid = (int)TransactionsStatusService.ETransactionsTypes.Pending;

                        transactionsReminders = DependencyConfig.TransactionsRemindersService.Update(transactionsReminders);

                        foreach (var splits in DependencyConfig.SplitsService.GetbyTransactionid(transactions.Id))
                        {
                            SplitsReminders splitsReminders = new();
                            splitsReminders.Transactionid = transactionsReminders.Id;
                            splitsReminders.Categoryid = splits.Categoryid;
                            splitsReminders.Memo = splits.Memo;
                            splitsReminders.AmountIn = splits.AmountIn;
                            splitsReminders.AmountOut = splits.AmountOut;
                            splitsReminders.Tagid = splits.Tagid;
                            DependencyConfig.SplitsRemindersService.Update(splitsReminders);
                        }

                        FrmTransactionReminders frm = new(transactionsReminders);
                        frm.ShowDialog();
                        if (frm.WindowsResult == EWindowsResult.Sucess)
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

        private void gvTransactions_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (gvTransactions.CurrentItem != null)
            {
                FrmTransaction frm = new((Transactions)gvTransactions.CurrentItem);
                frm.ShowDialog();
                LoadTransactions();
                parentForm.LoadAccounts();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (gvTransactions.SelectedItems != null && gvTransactions.SelectedItems.Count > 0)
            {
                foreach (Transactions transactions in gvTransactions.SelectedItems)
                {
                    RemoveTransaction(transactions);
                }
                LoadTransactions();
                parentForm.LoadAccounts();
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
                    transactions.TransactionStatusid = (int)TransactionsStatusService.ETransactionsTypes.Pending;
                    transactions.TransactionStatus = DependencyConfig.TransactionsStatusService.GetById(transactions.TransactionStatusid);
                    DependencyConfig.TransactionsService.Update(transactions);
                }
                LoadTransactions();
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
                    transactions.TransactionStatusid = (int)TransactionsStatusService.ETransactionsTypes.Provisional;
                    transactions.TransactionStatus = DependencyConfig.TransactionsStatusService.GetById(transactions.TransactionStatusid);
                    DependencyConfig.TransactionsService.Update(transactions);
                }
                LoadTransactions();
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
                    transactions.TransactionStatusid = (int)TransactionsStatusService.ETransactionsTypes.Reconciled;
                    transactions.TransactionStatus = DependencyConfig.TransactionsStatusService.GetById(transactions.TransactionStatusid);
                    DependencyConfig.TransactionsService.Update(transactions);
                }
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

        public void SetColumnVisibility(AccountsView? accountSelected = null)
        {
            TransactionViewModel.AccountsSelected = accountSelected;

            if (gvTransactions.View != null)
            {
                if (accountSelected != null)
                {
                    gvTransactions.Columns["Account.Description"].IsHidden = true;

                    if (TransactionViewModel.AccountsSelected.AccountsTypesid == (int)AccountsTypesService.EAccountsTypes.Invests)
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

        private void RemoveTransaction(Transactions transactions)
        {
            if (transactions.TranferSplitid != null)
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
                            DependencyConfig.TransactionsService.Delete(DependencyConfig.TransactionsService.GetById(splits.Tranferid));
                        }

                        DependencyConfig.SplitsService.Delete(splits);
                    }
                }

                if (transactions.Tranferid != null)
                {
                    DependencyConfig.TransactionsService.Delete(DependencyConfig.TransactionsService.GetById(transactions.Tranferid));
                }

                DependencyConfig.TransactionsService.Delete(transactions);
            }
        }


        #endregion       
    }
}
