using GARCA.Models;
using GARCA.Data.IOC;
using GARCA.View.Services;
using System.Windows;

namespace GARCA.View.Views
{
    /// <summary>
    /// Lógica de interacción para Splits.xaml
    /// </summary>
    public partial class FrmSplitsList : Window
    {
        private readonly Transactions? transactions;

        public FrmSplitsList()
        {
            InitializeComponent();
        }

        public FrmSplitsList(Transactions? transactions) :
            this()

        {
            this.transactions = transactions;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbCategories.ItemsSource = DependencyConfigView.CategoriesServiceView.GetAll();
            loadSplits();
        }

        private void loadSplits()
        {
            gvSplits.ItemsSource = transactions != null && transactions.Id > 0
                ? DependencyConfigView.SplitsServiceView.GetbyTransactionid(transactions.Id)
                : (object)DependencyConfigView.SplitsServiceView.GetbyTransactionidNull();
        }

        private void gvSplits_CurrentCellDropDownSelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellDropDownSelectionChangedEventArgs e)
        {
            var splits = (Splits)gvSplits.SelectedItem;
            if (splits != null)
            {
                switch (gvSplits.Columns[e.RowColumnIndex.ColumnIndex].MappingName)
                {
                    case "categoryid":
                        splits.Category = DependencyConfigView.CategoriesServiceView.GetById(splits.Categoryid);
                        break;
                }
            }
        }


        private void gvSplits_RowValidating(object sender, Syncfusion.UI.Xaml.Grid.RowValidatingEventArgs e)
        {
            var splits = (Splits)e.RowData;

            if (splits.Categoryid == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("Categoryid", "Tiene que rellenar el tipo de categoría");
            }
            else if (splits.Categoryid == (int)CategoriesServiceView.ESpecialCategories.Split)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("Categoryid", "No se puede utilizar esta categoría en un split");
            }

            if (splits.AmountIn == null && splits.AmountOut == null)
            {
                e.IsValid = false;
                e.ErrorMessages.Add("AmountIn", "Tiene que rellenar la cantidad");
                e.ErrorMessages.Add("AmountOut", "Tiene que rellenar la cantidad");
            }
        }

        private void gvSplits_RowValidated(object sender, Syncfusion.UI.Xaml.Grid.RowValidatedEventArgs e)
        {
            var splits = (Splits)e.RowData;

            DependencyConfigView.TransactionsServiceView.UpdateTranferSplits(transactions, ref splits);
            DependencyConfigView.SplitsServiceView.SaveChanges(splits);
        }

        private void gvSplits_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Splits splits in e.Items)
            {
                if (splits.Tranferid != null)
                {
                    DependencyConfigView.TransactionsServiceView.Delete(DependencyConfigView.TransactionsServiceView.GetById(splits.Tranferid));
                }
                DependencyConfigView.SplitsServiceView.Delete(splits);
            }

            loadSplits();
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
            var splits = (Splits)e.NewObject;
            splits.Transactionid = transactions.Id;
            splits.Transaction = transactions;
        }
    }
}
