using BBDDLib.Models;
using GastosRYC.BBDDLib.Services;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para FrmTransaction.xaml
    /// </summary>
    public partial class FrmTransaction : Window
    {

        #region Variables

        private Transactions? transaction;
        private readonly int? accountidDefault;
        private readonly SimpleInjector.Container servicesContainer;

        #endregion

        #region Constructor

        public FrmTransaction(SimpleInjector.Container servicesContainer)
        {
            InitializeComponent();
            this.servicesContainer = servicesContainer;
        }

        public FrmTransaction(Transactions transaction, int accountidDefault, SimpleInjector.Container servicesContainer) :
            this(servicesContainer)
        {
            this.transaction = transaction;
            this.accountidDefault = accountidDefault;
        }

        public FrmTransaction(Transactions transaction, SimpleInjector.Container servicesContainer) :
            this(servicesContainer)
        {
            this.transaction = transaction;
        }

        public FrmTransaction(int accountidDefault, SimpleInjector.Container servicesContainer) :
            this(servicesContainer)
        {
            this.accountidDefault = accountidDefault;
        }

        #endregion

        #region Eventos
        private void cbAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbAccount?.SelectedItem != null)
            {
                toggleViews(((Accounts)cbAccount.SelectedItem).accountsTypes);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadComboBox();
            loadTransaction();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (saveTransaction())
            {
                this.Close();
            }
        }

        private void btnSplit_Click(object sender, RoutedEventArgs e)
        {
            if ((cbCategory.SelectedValue == null) && (txtAmount.Value == null))
            {
                if(MessageBox.Show("Para hacer una división se tiene que asignar una categoría especial, ¿Esta de acuerdo?", "inserción movimiento", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    cbCategory.SelectedValue = (int)CategoriesService.eSpecialCategories.Split;
                    txtAmount.Value = 0;
                }
                else
                {
                    MessageBox.Show("Sin esta modificación no se puede modificar", "inserción movimiento");
                    return;
                }
            }

            if (transaction == null && !saveTransaction())
            {
                MessageBox.Show("Sin guardar no se puede realizar un split", "inserción movimiento");
                return;
            }

            FrmSplitsList frm = new FrmSplitsList(transaction, servicesContainer);
            frm.ShowDialog();
            servicesContainer.GetInstance<TransactionsService>().updateTransactionAfterSplits(transaction);
            loadTransaction();
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    if (saveTransaction())
                    {
                        DateTime previousDate = DateTime.Now;
                        if (dtpDate.SelectedDate != null)
                             previousDate = (DateTime)dtpDate.SelectedDate;
                        
                        transaction = null;
                        loadTransaction();

                        dtpDate.SelectedDate = previousDate;
                    }
                    break;
                case Key.F2:
                    saveTransaction();
                    break;
                case Key.Escape:
                    this.Close();
                    break;
            }
        }

        private void cbPerson_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPerson.SelectedItem != null)
            {
                Persons p = ((Persons)cbPerson.SelectedItem);
                if (p?.categoryid != null)
                {
                    cbCategory.SelectedValue = p.categoryid;
                }
            }
        }

        private void dtpDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                if (dtpDate.Text.Length == 4)
                {
                    dtpDate.Text = dtpDate.Text.Substring(0, 2) + "/" + dtpDate.Text.Substring(2, 2);
                }
                else if (dtpDate.Text.Length == 6)
                {
                    dtpDate.Text = dtpDate.Text.Substring(0, 2) + "/" + dtpDate.Text.Substring(2, 2) + "/" + dtpDate.Text.Substring(4, 2);
                }
            }
        }
        private void txtNumShares_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            calculateValueShares();
        }

        private void txtPriceShares_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            calculateValueShares();
        }

        #endregion

        #region Funtions

        private void toggleViews(AccountsTypes? accountsTypes)
        {
            if (accountsTypes != null)
            {
                switch (accountsTypes.id)
                {
                    case (int)AccountsTypesService.eAccountsTypes.Invests:
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
                        break;
                    default:
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
                        break;
                }
            }
        }

        private void loadTransaction()
        {
            if (transaction != null)
            {
                dtpDate.SelectedDate = transaction.date;
                cbAccount.SelectedValue = transaction.accountid;
                cbPerson.SelectedValue = transaction.personid;
                txtMemo.Text = transaction.memo;
                cbCategory.SelectedValue = transaction.categoryid;
                txtAmount.Value = transaction.amount;
                cbTag.SelectedValue = transaction.tagid;
                cbTransactionStatus.SelectedValue = transaction.transactionStatusid;
                cbInvestmentProduct.SelectedValue = transaction.investmentProductsid;
                txtNumShares.Value = Convert.ToDouble(transaction.numShares);
                txtPriceShares.Value = transaction.pricesShares;
            }
            else
            {

                dtpDate.SelectedDate = DateTime.Now;

                if (accountidDefault != null)
                {
                    cbAccount.SelectedValue = accountidDefault;
                }
                else
                {
                    cbAccount.SelectedValue = null;
                }

                cbPerson.SelectedValue = null;
                cbCategory.SelectedValue = null;
                txtMemo.Text = null;
                txtAmount.Value = null;
                cbTag.SelectedValue = null;
                cbInvestmentProduct.SelectedValue = null;
                txtNumShares.Value = null;
                txtPriceShares.Value = null;
                cbTransactionStatus.SelectedValue = (int)TransactionsStatusService.eTransactionsTypes.Pending;

                dtpDate.Focus();
            }
        }

        private void updateTransaction()
        {
            if (transaction == null)
            {
                transaction = new Transactions();
            }

            transaction.date = dtpDate.SelectedDate;
            transaction.accountid = (int)cbAccount.SelectedValue;
            transaction.account = servicesContainer.GetInstance<AccountsService>().getByID(transaction.accountid);

            if (cbPerson.SelectedValue != null)
            {
                transaction.personid = (int)cbPerson.SelectedValue;
                transaction.person = servicesContainer.GetInstance<PersonsService>().getByID(transaction.personid);
            }

            transaction.memo = txtMemo.Text;
            if (cbCategory.SelectedValue == null && cbAccount?.SelectedItem != null &&
                ((Accounts)cbAccount.SelectedItem).accountsTypesid == (int)AccountsTypesService.eAccountsTypes.Invests)
            {
                cbCategory.SelectedValue = 0;
            }

            if (cbCategory.SelectedValue != null)
            {
                transaction.categoryid = (int)cbCategory.SelectedValue;
                transaction.category = servicesContainer.GetInstance<CategoriesService>().getByID(transaction.categoryid);
            }

            if (cbInvestmentProduct.SelectedValue != null)
            {
                transaction.investmentProductsid = (int)cbInvestmentProduct.SelectedValue;
                transaction.investmentProducts = servicesContainer.GetInstance<InvestementProductsService>().getByID(transaction.investmentProductsid);
            }

            if(((Accounts)cbAccount.SelectedItem).accountsTypesid == (int)AccountsTypesService.eAccountsTypes.Invests)
            {
                transaction.numShares = (decimal?)Convert.ToDouble(txtNumShares.Value ?? 0);
                transaction.pricesShares = txtPriceShares.Value ?? 0;
            }
            else
            {
                transaction.numShares = (decimal?)Convert.ToDouble(txtNumShares.Value);
                transaction.pricesShares = txtPriceShares.Value;
            }

            if (txtAmount.Value > 0)
            {
                transaction.amountIn = txtAmount.Value;
                transaction.amountOut = 0;
            }
            else
            {
                transaction.amountOut = -txtAmount.Value;
                transaction.amountIn = 0;
            }

            if (cbTag.SelectedValue != null)
            {
                transaction.tagid = (int)cbTag.SelectedValue;
                transaction.tag = servicesContainer.GetInstance<TagsService>().getByID(transaction.tagid);
            }

            transaction.transactionStatusid = (int)cbTransactionStatus.SelectedValue;
            transaction.transactionStatus = servicesContainer.GetInstance<TransactionsStatusService>().getByID(transaction.transactionStatusid);
        }

        private void loadComboBox()
        {
            cbAccount.ItemsSource = servicesContainer.GetInstance<AccountsService>().getAll();
            cbPerson.ItemsSource = servicesContainer.GetInstance<PersonsService>().getAll();
            cbCategory.ItemsSource = servicesContainer.GetInstance<CategoriesService>().getAll();
            cbInvestmentProduct.ItemsSource = servicesContainer.GetInstance<InvestementProductsService>().getAll();
            cbTag.ItemsSource = servicesContainer.GetInstance<TagsService>().getAll();
            cbTransactionStatus.ItemsSource = servicesContainer.GetInstance<TransactionsStatusService>().getAll();
        }


        private bool isTransactionValid()
        {
            String errorMessage = "";
            bool valid = true;

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

            if (cbCategory.SelectedValue == null && cbAccount?.SelectedItem != null && 
                ((Accounts)cbAccount.SelectedItem).accountsTypesid != (int)AccountsTypesService.eAccountsTypes.Invests)
            {
                errorMessage += "- Categoría\n";
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

            if(transaction?.tranferSplitid != null)
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



        private bool saveTransaction()
        {
            if (isTransactionValid())
            {
                if (MessageBox.Show("Se va a proceder a guardar el movimiento", "inserción movimiento", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    updateTransaction();
                    if (transaction != null)
                    {
                        servicesContainer.GetInstance<TransactionsService>().saveChanges(transaction);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void calculateValueShares()
        {
            if (txtNumShares.Value != null && txtPriceShares.Value != null)
            {
                txtAmount.Value = (Decimal?)Convert.ToDouble(txtNumShares.Value) * txtPriceShares.Value;
            }
        }

        #endregion

    }
}
