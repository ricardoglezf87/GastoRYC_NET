using BOLib.Models;
using BOLib.ModelsView;
using BOLib.Services;
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

namespace GastosRYC.ViewModels
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
            IncrementalItemsSource = new IncrementalList<Transactions>(LoadMoreItems) { MaxItemCount = 50 };
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
                item = await Task.Run(() => TransactionsService.Instance.getByAccountOrderByOrderDesc(accountsSelected.id));
            }
            else
            {
                item = await Task.Run(() => TransactionsService.Instance.getAllOpennedOrderByOrderDesc());
            }
            if (item != null)
            {
                var transactions = new ObservableCollection<Transactions?>(item);
                var list = transactions.Skip(baseIndex).Take(50).ToList();
                await Task.Delay(1000); //TODO: Es un chapuza, pero no se porque da error, probar si es fallo de version.
                IncrementalItemsSource.LoadItems(list);
            }
        }
    }
}
