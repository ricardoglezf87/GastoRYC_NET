using BBDDLib.Models;
using GastosRYC.BBDDLib.Services;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para Splits.xaml
    /// </summary>
    public partial class frmSplits : Window
    {

        private readonly CategoriesService categoriesService = new CategoriesService();
        private readonly SplitsService splitsService = new SplitsService();
        private readonly TransactionsService transactionsService = new TransactionsService();
        private readonly Transactions? transactions;

        public frmSplits(Transactions? transactions)
        {
            InitializeComponent();
            this.transactions = transactions;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbCategories.ItemsSource = categoriesService.getAll();
            if (transactions != null && transactions.id > 0)
            {
                gvSplits.ItemsSource = splitsService.getbyTransactionid(transactions.id);
            }
            else
            {
                gvSplits.ItemsSource = splitsService.getbyTransactionidNull();
            }
        }

        private void gvSplits_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            Splits splits = (Splits)gvSplits.SelectedItem;
            if (splits != null)
            {
                switch (gvSplits.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "categoryid":
                        splits.category = categoriesService.getByID(splits.categoryid);
                        break;                    
                }
            }
        }

       
        private void gvSplits_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            Splits splits = (Splits)e.RowData;
            
            if (splits.categoryid == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("categoryid", "Tiene que rellenar el tipo de categoría");
            }
            else if(splits.categoryid == (int)CategoriesService.eSpecialCategories.Split)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("categoryid", "No se puede utilizar esta categoría en un split");
            }

            if (splits.amountIn == null && splits.amountOut == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("amountIn", "Tiene que rellenar la cantidad");
                e.ErrorMessages.Add("amountOut", "Tiene que rellenar la cantidad");
            }
        }                
        
        private void gvSplits_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            Splits splits = (Splits)e.RowData;

            saveChanges(splits);
        }

        private void saveChanges(Splits splits)
        {
            if (splits.category == null && splits.categoryid != null)
            {
                splits.category = categoriesService.getByID(splits.categoryid);
            }

            if (splits.amountIn == null)
                splits.amountIn = 0;

            if (splits.amountOut == null)
                splits.amountOut = 0;

            updateTranfer(splits);

            splitsService.update(splits);
        }

        private void updateTranfer(Splits splits)
        {
            if (splits.tranferid != null && 
                splits.category.categoriesTypesid != (int)CategoriesService.eCategoriesTypes.Transfers)
            {
                Transactions? tContraria = transactionsService.getByID(splits.tranferid);
                if (tContraria != null)
                {
                    transactionsService.delete(tContraria);
                }
                splits.tranferid = null;
            }
            else if (splits.tranferid == null && 
                splits.category.categoriesTypesid == (int)CategoriesService.eCategoriesTypes.Transfers)
            {
                splits.tranferid = transactionsService.getNextID();

                Transactions? tContraria = new Transactions();
                tContraria.date = transactions.date;
                tContraria.accountid = splits.category.accounts.id;
                tContraria.personid = transactions.personid;
                tContraria.categoryid = transactions.account.categoryid;
                tContraria.memo = splits.memo;
                tContraria.tagid = transactions.tagid;
                tContraria.amountIn = splits.amountOut;
                tContraria.amountOut = splits.amountIn;

                if (splits.id != 0)
                    tContraria.tranferSplitid = splits.id;
                else
                    tContraria.tranferSplitid = splitsService.getNextID() + 1;

                tContraria.transactionStatusid = transactions.transactionStatusid;

                transactionsService.update(tContraria);

            }
            else if (splits.tranferid != null && 
                splits.category.categoriesTypesid == (int)CategoriesService.eCategoriesTypes.Transfers)
            {
                Transactions? tContraria = transactionsService.getByID(splits.tranferid);
                if (tContraria != null)
                {
                    tContraria.date = transactions.date;
                    tContraria.accountid = splits.category.accounts.id;
                    tContraria.personid = transactions.personid;
                    tContraria.categoryid = transactions.account.categoryid;
                    tContraria.memo = splits.memo;
                    tContraria.tagid = transactions.tagid;
                    tContraria.amountIn = splits.amountOut??0;
                    tContraria.amountOut = splits.amountIn??0;
                    tContraria.transactionStatusid = transactions.transactionStatusid;
                    transactionsService.update(tContraria);
                }
            }
        }


        private void gvSplits_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Splits splits in e.Items) {
                if (splits.tranferid != null)
                {
                    transactionsService.delete(transactionsService.getByID(splits.tranferid));
                }
                splitsService.delete(splits);
            }            
        }

        private void gvSplits_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if(MessageBox.Show("Esta seguro de querer eliminar esta split?","Eliminación split",MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation,MessageBoxResult.No) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void gvSplits_AddNewRowInitiating(object sender, Syncfusion.UI.Xaml.Grid.AddNewRowInitiatingEventArgs e)
        {
            Splits splits = (Splits)e.NewObject;
            splits.transactionid = transactions.id;
            splits.transaction = transactions;
        }
    }
}
