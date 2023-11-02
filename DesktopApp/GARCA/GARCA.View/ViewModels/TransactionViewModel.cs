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
        public static Accounts? AccountsSelected;
        public static int maxItem;

        public IncrementalList<Transactions>? IncrementalItemsSource { get; set; }

        public TransactionViewModel()
        {
            if (maxItem == 0)
            {
                maxItem = 200;
            }

            IncrementalItemsSource = new IncrementalList<Transactions>(LoadMoreItems) { MaxItemCount = maxItem };
        }

        /// <summary>
        /// Method to load items which assigned to the action of IncrementalList
        /// </summary>
        /// <param name="count"></param>
        /// <param name="baseIndex"></param>
        private async void LoadMoreItems(uint count, int baseIndex)
        {
            if (baseIndex % 50 != 0)
            {
                return;
            }

            IncrementalItemsSource.Clear();
            var item = AccountsSelected != null
                ? await Task.Run(() => iTransactionsService.GetByAccountOrderByOrderDesc(AccountsSelected.Id, baseIndex, maxItem))
                : await Task.Run(() => iTransactionsService.GetAllOpennedOrderByOrderDesc(baseIndex, maxItem));

            if (item != null)
            {
                var transactions = new ObservableCollection<Transactions>(item);
                IncrementalItemsSource.LoadItems(transactions.AsEnumerable());
            }
        }
    }
}
