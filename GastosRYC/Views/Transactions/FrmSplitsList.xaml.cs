using BOLib.Models;
using BOLib.Services;
using System.Windows;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para Splits.xaml
    /// </summary>
    public partial class FrmSplitsList : Window
    {
        private readonly Transactions? transactions;
        private readonly TransactionsService transactionsService;
        private readonly CategoriesService categoriesService;
        private readonly SplitsService splitsService;

        public FrmSplitsList()
        {
            InitializeComponent();
            categoriesService = InstanceBase<CategoriesService>.Instance;
            splitsService = InstanceBase<SplitsService>.Instance;
            transactionsService = InstanceBase<TransactionsService>.Instance;
        }

        public FrmSplitsList(Transactions? transactions) :
            this()

        {
            this.transactions = transactions;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbCategories.ItemsSource = categoriesService.getAll();
            gvSplits.ItemsSource = transactions != null && transactions.id > 0
                ? splitsService.getbyTransactionid(transactions.id)
                : (object?)splitsService.getbyTransactionidNull();
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
            else if (splits.categoryid == (int)CategoriesService.eSpecialCategories.Split)
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

            splitsService.saveChanges(transactions, splits);
            transactionsService.updateTranferSplits(transactions, splits);
        }

        private void gvSplits_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Splits splits in e.Items)
            {
                if (splits.tranferid != null)
                {
                    transactionsService.delete(transactionsService.getByID(splits.tranferid));
                }
                splitsService.delete(splits);
            }
        }

        private void gvSplits_RecordDeleting(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletingEventArgs e)
        {
            if (MessageBox.Show("Esta seguro de querer eliminar esta split?", "Eliminación split", MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation, MessageBoxResult.No) == MessageBoxResult.No)
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
