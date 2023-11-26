using Syncfusion.UI.Xaml.Grid;
using System.Threading.Tasks;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.View.ViewModels
{
    public class TransactionViewModel
    {
        private GridVirtualizingCollectionView source;

        public TransactionViewModel()
        {
            LoadData();
        }

        public GridVirtualizingCollectionView GetSource()
        {
            return source;
        }

        public async Task LoadData()
        {
            var item = await iTransactionsService.GetAllOpennedOrderByOrdenDesc();

            if (item != null)
            {
                source = new GridVirtualizingCollectionView(item);
            }
        }

        public void Refresh()
        {
            source.Refresh();
        }
    }
}
