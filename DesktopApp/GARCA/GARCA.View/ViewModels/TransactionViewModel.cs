using GARCA.Models;
using Syncfusion.UI.Xaml.Grid;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.View.ViewModels
{
    public class TransactionViewModel
    {
        public GridVirtualizingCollectionView GridVirtualizingItemsSource { get; set; }

        public TransactionViewModel()
        {
            var item = iTransactionsService.GetAllOpennedOrderByOrdenDesc().Result;

            if (item != null)
            {
                GridVirtualizingItemsSource = new GridVirtualizingCollectionView(item);
            }
        }
    }
}
