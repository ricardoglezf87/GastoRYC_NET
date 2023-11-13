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
        public IncrementalList<Transactions>? IncrementalItemsSource { get; set; }
        public static Accounts? AccountsSelected { get; set; }
        public static int MaxItem { get; set; } = 200;

        public TransactionViewModel()
        {
            IncrementalItemsSource = new IncrementalList<Transactions>(LoadMoreItems) { MaxItemCount = MaxItem };
        }

        /// <summary>
        /// Method to load items which assigned to the action of IncrementalList
        /// </summary>
        /// <param name="count"></param>
        /// <param name="baseIndex"></param>
        private async void LoadMoreItems(uint count, int baseIndex)
        {
            var item = AccountsSelected != null
                ? await Task.Run(() => iTransactionsService.GetByAccountOrderByOrderDesc(AccountsSelected.Id, baseIndex, MaxItem))
                : await Task.Run(() => iTransactionsService.GetAllOpennedOrderByOrdenDesc(baseIndex, MaxItem));

            if (item != null)
            {
                var transactions = new ObservableCollection<Transactions>(item);
                IncrementalItemsSource.LoadItems(transactions.AsEnumerable());
            }
        }
    }
}
