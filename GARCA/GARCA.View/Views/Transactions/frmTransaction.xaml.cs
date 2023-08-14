using GARCA.BO.Models;
using GARCA.BO.Services;
using GARCA.Utils.IOC;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GARCA.View.Views
{
    /// <summary>
    /// Lógica de interacción para FrmTransaction.xaml
    /// </summary>
    public partial class FrmTransaction : Window
    {

        #region Variables

        private Transactions? transaction;
        private readonly int? accountidDefault;

        #endregion

        #region Constructor

        public FrmTransaction()
        {
            InitializeComponent();
        }

        public FrmTransaction(Transactions transaction, int accountidDefault) :
            this()
        {
            this.transaction = transaction;
            this.accountidDefault = accountidDefault;
        }

        public FrmTransaction(Transactions transaction) :
            this()
        {
            this.transaction = transaction;
        }

        public FrmTransaction(int accountidDefault) :
            this()
        {
            this.accountidDefault = accountidDefault;
        }

        #endregion

        #region Eventos
        private void cbAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbAccount?.SelectedItem != null && !transaction.InvestmentCategory.HasValue)
            {
                transaction.InvestmentCategory = ((Accounts)cbAccount.SelectedItem).AccountsTypesid !=
                    (int)AccountsTypesService.EAccountsTypes.Invests;
            }
            ToggleViews();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComboBox();
            LoadTransaction();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (SaveTransaction())
            {
                Close();
            }
        }

        private void btnSplit_Click(object sender, RoutedEventArgs e)
        {
            if (cbCategory.SelectedValue == null && txtAmount.Value == null)
            {
                if (MessageBox.Show("Para hacer una división se tiene que asignar una categoría especial, ¿Esta de acuerdo?", "inserción movimiento", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    cbCategory.SelectedValue = (int)CategoriesService.ESpecialCategories.Split;
                    txtAmount.Value = 0;
                }
                else
                {
                    MessageBox.Show("Sin esta modificación no se puede modificar", "inserción movimiento");
                    return;
                }
            }

            if (transaction == null || !SaveTransaction())
            {
                MessageBox.Show("Sin guardar no se puede realizar un split", "inserción movimiento");
                return;
            }

            FrmSplitsList frm = new(transaction);
            frm.ShowDialog();
            DependencyConfig.TransactionsService.UpdateTransactionAfterSplits(transaction);
            LoadTransaction();
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    if (SaveTransaction())
                    {
                        var previousDate = DateTime.Now;
                        if (dtpDate.SelectedDate != null)
                        {
                            previousDate = (DateTime)dtpDate.SelectedDate;
                        }
                        var investmentCategory = transaction.InvestmentCategory;

                        transaction = null;
                        LoadTransaction();

                        transaction.InvestmentCategory = investmentCategory;
                        dtpDate.SelectedDate = previousDate;
                    }
                    break;
                case Key.F2:
                    SaveTransaction();
                    break;
                case Key.F3:
                    transaction.InvestmentCategory = !transaction.InvestmentCategory ?? false;
                    ToggleViews();
                    break;
                case Key.F4:
                    CalculateSharesOrPrice();
                    break;
                case Key.Escape:
                    Close();
                    break;
            }
        }

        private void cbPerson_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPerson.SelectedItem != null)
            {
                var p = (Persons)cbPerson.SelectedItem;
                if (p?.Categoryid != null)
                {
                    cbCategory.SelectedValue = p.Categoryid;
                }
            }
        }

        private void dtpDate_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Tab or Key.Enter)
            {
                if (dtpDate.Text.Length == 4)
                {
                    dtpDate.Text = dtpDate.Text.Substring(0, 2) + "/" + dtpDate.Text.Substring(2, 2);
                }
                else if (dtpDate.Text.Length == 6)
                {
                    dtpDate.Text = dtpDate.Text.Substring(0, 2) + "/" + dtpDate.Text.Substring(2, 2) + "/" + dtpDate.Text.Substring(4, 2);
                }

                if (e.Key == Key.Enter)
                {
                    cbAccount.Focus();
                }
            }
        }

        private void txtNumShares_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalculateValueShares();
        }

        private void txtPriceShares_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalculateValueShares();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DependencyConfig.TransactionsService.RefreshBalanceTransactions(transaction);
        }

        #endregion

        #region Funtions

        private void CalculateSharesOrPrice()
        {
            if (txtNumShares.Value != null)
            {
                CalculatePriceByShares();
            }
            else if (txtPriceShares.Value != null)
            {
                CalculateSharesByPrice();
            }
            else
            {
                MessageBox.Show("debe poner una cantidad de participaciones o un precio", "Trasacción");
            }
        }

        private void CalculatePriceByShares()
        {
            var importe = Microsoft.VisualBasic.Interaction.InputBox("Inserte un importe:", "Transacción");
            if (!String.IsNullOrWhiteSpace(importe))
            {
                var aux = Double.Parse(importe.Replace(".", ",")) / txtNumShares.Value;
                txtPriceShares.Value = (decimal?)aux;
            }
        }

        private void CalculateSharesByPrice()
        {
            var importe = Microsoft.VisualBasic.Interaction.InputBox("Inserte un importe:", "Transacción");
            if (!String.IsNullOrWhiteSpace(importe))
            {
                var aux = Decimal.Parse(importe.Replace(".", ",")) / txtPriceShares.Value;
                txtNumShares.Value = (double?)aux;
            }
        }

        private void ToggleViews()
        {
            if (transaction.InvestmentCategory == false)
            {
                lblInvestmentProduct.Visibility = Visibility.Visible;
                cbInvestmentProduct.Visibility = Visibility.Visible;
                lblNumShares.Visibility = Visibility.Visible;
                txtNumShares.Visibility = Visibility.Visible;
                lblPriceShares.Visibility = Visibility.Visible;
                txtPriceShares.Visibility = Visibility.Visible;
                lblPerson.Visibility = Visibility.Hidden;
                cbPerson.Visibility = Visibility.Hidden;
                lblCategory.Visibility = Visibility.Hidden;
                cbCategory.Visibility = Visibility.Hidden;
                lblTag.Visibility = Visibility.Hidden;
                cbTag.Visibility = Visibility.Hidden;
                Grid.SetRow(lblMemo, 6);
                Grid.SetRow(txtMemo, 6);
                CalculateValueShares();
            }
            else
            {
                lblInvestmentProduct.Visibility = Visibility.Hidden;
                cbInvestmentProduct.Visibility = Visibility.Hidden;
                lblNumShares.Visibility = Visibility.Hidden;
                txtNumShares.Visibility = Visibility.Hidden;
                lblPriceShares.Visibility = Visibility.Hidden;
                txtPriceShares.Visibility = Visibility.Hidden;
                lblPerson.Visibility = Visibility.Visible;
                cbPerson.Visibility = Visibility.Visible;
                lblCategory.Visibility = Visibility.Visible;
                cbCategory.Visibility = Visibility.Visible;
                lblTag.Visibility = Visibility.Visible;
                cbTag.Visibility = Visibility.Visible;
                Grid.SetRow(lblMemo, 4);
                Grid.SetRow(txtMemo, 4);
            }
        }

        private void LoadTransaction()
        {
            if (transaction != null)
            {
                dtpDate.SelectedDate = transaction.Date;
                cbAccount.SelectedValue = transaction.Accountid;
                cbPerson.SelectedValue = transaction.Personid;
                txtMemo.Text = transaction.Memo;
                cbCategory.SelectedValue = transaction.Categoryid;
                txtAmount.Value = transaction.Amount;
                cbTag.SelectedValue = transaction.Tagid;
                cbTransactionStatus.SelectedValue = transaction.TransactionStatusid;
                cbInvestmentProduct.SelectedValue = transaction.InvestmentProductsid;
                txtNumShares.Value = Convert.ToDouble(transaction.NumShares);
                txtPriceShares.Value = transaction.PricesShares;
            }
            else
            {
                transaction = new Transactions();
                dtpDate.SelectedDate = DateTime.Now;

                cbAccount.SelectedValue = accountidDefault != null ? accountidDefault : (object?)null;

                cbPerson.SelectedIndex = -1;
                cbPerson.Text = String.Empty;
                cbCategory.SelectedIndex = -1;
                cbCategory.Text = String.Empty;
                txtMemo.Text = null;
                txtAmount.Value = null;
                cbTag.SelectedIndex = -1;
                cbTag.Text = String.Empty; cbTransactionStatus.SelectedIndex = -1;
                cbInvestmentProduct.SelectedIndex = -1;
                cbInvestmentProduct.Text = String.Empty;
                txtNumShares.Value = null;
                txtPriceShares.Value = null;
                cbTransactionStatus.SelectedValue = (int)TransactionsStatusService.ETransactionsTypes.Pending;

                dtpDate.Focus();
            }
        }

        private void UpdateTransaction()
        {
            transaction ??= new Transactions();

            transaction.Date = dtpDate.SelectedDate;
            transaction.Accountid = (int)cbAccount.SelectedValue;
            transaction.Account = DependencyConfig.AccountsService.GetById(transaction.Accountid);

            if (cbPerson.SelectedValue != null)
            {
                transaction.Personid = (int)cbPerson.SelectedValue;
                transaction.Person = DependencyConfig.PersonsService.GetById(transaction.Personid);
            }

            transaction.Memo = txtMemo.Text;
            if (cbCategory.SelectedValue == null && cbAccount?.SelectedItem != null &&
                ((Accounts)cbAccount.SelectedItem).AccountsTypesid == (int)AccountsTypesService.EAccountsTypes.Invests)
            {
                cbCategory.SelectedValue = 0;
            }

            if (cbCategory.SelectedValue != null)
            {
                transaction.Categoryid = (int)cbCategory.SelectedValue;
                transaction.Category = DependencyConfig.CategoriesService.GetById(transaction.Categoryid);
            }

            if (cbInvestmentProduct.SelectedValue != null)
            {
                transaction.InvestmentProductsid = (int)cbInvestmentProduct.SelectedValue;
                transaction.InvestmentProducts = DependencyConfig.InvestmentProductsService.GetById(transaction.InvestmentProductsid);
            }

            transaction.NumShares = (decimal?)Convert.ToDouble(txtNumShares.Value);
            transaction.PricesShares = txtPriceShares.Value ?? 0;

            if (txtAmount.Value > 0)
            {
                transaction.AmountIn = txtAmount.Value;
                transaction.AmountOut = 0;
            }
            else
            {
                transaction.AmountOut = -txtAmount.Value;
                transaction.AmountIn = 0;
            }

            if (cbTag.SelectedValue != null)
            {
                transaction.Tagid = (int)cbTag.SelectedValue;
                transaction.Tag = DependencyConfig.TagsService.GetById(transaction.Tagid);
            }

            transaction.TransactionStatusid = (int)cbTransactionStatus.SelectedValue;
            transaction.TransactionStatus = DependencyConfig.TransactionsStatusService.GetById(transaction.TransactionStatusid);
        }

        private void LoadComboBox()
        {
            cbAccount.ItemsSource = DependencyConfig.AccountsService.GetAll();
            cbPerson.ItemsSource = DependencyConfig.PersonsService.GetAll();
            cbCategory.ItemsSource = DependencyConfig.CategoriesService.GetAll();
            cbInvestmentProduct.ItemsSource = DependencyConfig.InvestmentProductsService.GetAll();
            cbTag.ItemsSource = DependencyConfig.TagsService.GetAll();
            cbTransactionStatus.ItemsSource = DependencyConfig.TransactionsStatusService.GetAll();
        }

        private bool IsTransactionValid()
        {
            var errorMessage = "";
            var valid = true;

            if (dtpDate.SelectedDate == null)
            {
                errorMessage += "- Fecha\n";
                valid = false;
            }

            if (cbAccount.SelectedValue == null)
            {
                errorMessage += "- Cuenta\n";
                valid = false;
            }
            else if (((Accounts)cbAccount.SelectedItem).AccountsTypesid != (int)AccountsTypesService.EAccountsTypes.Invests &&
                    transaction.InvestmentCategory == false)
            {
                errorMessage += "- No se puede realizar una transacción de inversión en una cuenta que no sea de inversión\n";
                valid = false;
            }

            if (cbCategory.SelectedValue == null && (!transaction.InvestmentCategory.HasValue || transaction.InvestmentCategory == true))
            {
                errorMessage += "- Categoría\n";
                valid = false;
            }

            if (cbInvestmentProduct.SelectedValue == null && transaction.InvestmentCategory == false)
            {
                errorMessage += "- Producto de inversión\n";
                valid = false;
            }

            if (txtPriceShares.Value < 0)
            {
                errorMessage += "- El precio de las participaciones no puede ser negativo, valore poner las participaciones en negativo\n";
                valid = false;
            }

            if (txtAmount.Value == null)
            {
                errorMessage += "- Cantidad\n";
                valid = false;
            }

            if (cbTransactionStatus.SelectedValue == null)
            {
                errorMessage += "- Estado\n";
                valid = false;
            }

            if (transaction?.TranferSplitid != null)
            {
                errorMessage += "- No se puede editar una transferencia proveniente de un split\n";
                valid = false;
            }

            if (errorMessage != "")
            {
                errorMessage = "Tiene que rellenar los siguiente campos para continuar:\n" + errorMessage.TrimEnd('\n');
                MessageBox.Show(errorMessage, "Error de validación", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return valid;
        }

        private bool SaveTransaction()
        {
            if (IsTransactionValid())
            {
                if (MessageBox.Show("Se va a proceder a guardar el movimiento", "inserción movimiento", MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    UpdateTransaction();
                    if (transaction != null)
                    {
                        DependencyConfig.TransactionsService.SaveChanges(ref transaction);
                    }
                    return true;
                }

                return false;
            }

            return false;
        }

        private void CalculateValueShares()
        {
            if (txtNumShares.Value != null && txtPriceShares.Value != null
                && transaction != null && transaction.InvestmentCategory.HasValue
                && transaction.InvestmentCategory.Value == false)
            {
                txtAmount.Value = (Decimal?)Convert.ToDouble(txtNumShares.Value) * txtPriceShares.Value;
            }
        }

        #endregion        
    }
}
