using GARCA.BO.Models;
using GARCA.BO.ModelsView;
using GARCA.BO.Services;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GARCA.ViewModels
{
    public class TransactionViewModel
    {
        public static AccountsView? accountsSelected;

        private IncrementalList<Transactions>? _incrementalItemsSource;
        public IncrementalList<Transactions>? IncrementalItemsSource
        {
            get { return _incrementalItemsSource; }
            set { _incrementalItemsSource = value; }
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
            List<Transactions?>? item;
            if (accountsSelected != null)
            {
                item = await Task.Run(() => TransactionsService.Instance.getByAccountOrderByOrderDesc(accountsSelected.id, baseIndex,50));
            }
            else
            {
                item = await Task.Run(() => TransactionsService.Instance.getAllOpennedOrderByOrderDesc(baseIndex, 50));
            }
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
