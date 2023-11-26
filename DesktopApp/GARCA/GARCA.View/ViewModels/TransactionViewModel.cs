using Syncfusion.UI.Xaml.Grid;
using System.Threading.Tasks;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.View.ViewModels
{
    public class TransactionViewModel
    {
        private GridVirtualizingCollectionView? source;

        public TransactionViewModel()
        {
            source = null;
        }

        public async Task<GridVirtualizingCollectionView?> GetSource()
        {
            if (source == null)
            {
                await LoadData();
            }

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
