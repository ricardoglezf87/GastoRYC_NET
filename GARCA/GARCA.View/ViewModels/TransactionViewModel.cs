using GARCA.BO.Models;
using GARCA.View.ViewModels;
using GARCA.BO.Services;
using Syncfusion.UI.Xaml.Grid;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GARCA.Utils.IOC;

namespace GARCA.View.ViewModels
{
    public class TransactionViewModel
    {
        public static AccountsView? AccountsSelected;

        public IncrementalList<Transactions>? IncrementalItemsSource { get; set; }

        public TransactionViewModel()
        {
            IncrementalItemsSource = new IncrementalList<Transactions>(LoadMoreItems) { MaxItemCount = 100 };
        }

        /// <summary>
        /// Method to load items which assigned to the action of IncrementalList
        /// </summary>
        /// <param name="count"></param>
        /// <param name="baseIndex"></param>
        private async void LoadMoreItems(uint count, int baseIndex)
        {
            var item = AccountsSelected != null
                ? await Task.Run(() => DependencyConfig.ITransactionsService.GetByAccountOrderByOrderDesc(AccountsSelected.Id, baseIndex, 50))
                : await Task.Run(() => DependencyConfig.ITransactionsService.GetAllOpennedOrderByOrderDesc(baseIndex, 50));
            if (item != null)
            {
                var transactions = new ObservableCollection<Transactions?>(item);
                IncrementalItemsSource.LoadItems(transactions);
            }
        }
    }
}
