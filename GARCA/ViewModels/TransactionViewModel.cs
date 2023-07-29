using GARCA.BO.Models;
using GARCA.BO.ModelsView;
using GARCA.BO.Services;
using Syncfusion.UI.Xaml.Grid;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GARCA.IOC;

namespace GARCA.ViewModels
{
    public class TransactionViewModel
    {
        public static AccountsView? accountsSelected;

        private IncrementalList<Transactions>? _incrementalItemsSource;
        public IncrementalList<Transactions>? IncrementalItemsSource
        {
            get => _incrementalItemsSource;
            set => _incrementalItemsSource = value;
        }

        public TransactionViewModel()
        {
            IncrementalItemsSource = new IncrementalList<Transactions>(LoadMoreItems) { MaxItemCount = 100 };
        }

        /// <summary>
        /// Method to load items which assigned to the action of IncrementalList
        /// </summary>
        /// <param name="count"></param>
        /// <param name="baseIndex"></param>
        async void LoadMoreItems(uint count, int baseIndex)
        {
            List<Transactions?>? item = accountsSelected != null
                ? await Task.Run(() => DependencyConfig.iTransactionsService.getByAccountOrderByOrderDesc(accountsSelected.id, baseIndex, 50))
                : await Task.Run(() => DependencyConfig.iTransactionsService.getAllOpennedOrderByOrderDesc(baseIndex, 50));
            if (item != null)
            {
                var transactions = new ObservableCollection<Transactions?>(item);
                if (transactions != null)
                {
                    IncrementalItemsSource.LoadItems(transactions);
                }
            }
        }
    }
}
