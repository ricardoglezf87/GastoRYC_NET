using BBDDLib.Models;
using GastosRYC.BBDDLib.Services;
using System.Windows;

namespace GastosRYC.Views
{
    /// <summary>
    /// Lógica de interacción para Splits.xaml
    /// </summary>
    public partial class FrmSplitsList : Window
    {
        private readonly Transactions? transactions;
        private readonly SimpleInjector.Container servicesContainer;

        public FrmSplitsList(SimpleInjector.Container servicesContainer)
        {
            InitializeComponent();
            this.servicesContainer = servicesContainer;
        }

        public FrmSplitsList(Transactions? transactions, SimpleInjector.Container servicesContainer) :
            this(servicesContainer)

        {
            this.transactions = transactions;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbCategories.ItemsSource = servicesContainer.GetInstance<ICategoriesService>().getAll();
            if (transactions != null && transactions.id > 0)
            {
                gvSplits.ItemsSource = servicesContainer.GetInstance<ISplitsService>().getbyTransactionid(transactions.id);
            }
            else
            {
                gvSplits.ItemsSource = servicesContainer.GetInstance<ISplitsService>().getbyTransactionidNull();
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
                        splits.category = servicesContainer.GetInstance<ICategoriesService>().getByID(splits.categoryid);
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
            else if (splits.categoryid == (int)ICategoriesService.eSpecialCategories.Split)
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

            servicesContainer.GetInstance<ISplitsService>().saveChanges(transactions, splits);
        }

        private void gvSplits_RecordDeleted(object sender, Syncfusion.UI.Xaml.Grid.RecordDeletedEventArgs e)
        {
            foreach (Splits splits in e.Items)
            {
                if (splits.tranferid != null)
                {
                    servicesContainer.GetInstance<ITransactionsService>().delete(servicesContainer.GetInstance<ITransactionsService>().getByID(splits.tranferid));
                }
                servicesContainer.GetInstance<ISplitsService>().delete(splits);
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
