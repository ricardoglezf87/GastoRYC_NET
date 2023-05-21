using BOLib.Extensions;

using BOLib.Models;
using DAOLib.Managers;
using System;
using System.Collections.Generic;

namespace BOLib.Services
{
    public class SplitsService
    {

        private readonly SplitsManager splitsManager;
        private static SplitsService? _instance;
        private static readonly object _lock = new object();

        public static SplitsService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new SplitsService();
                    }
                }
                return _instance;
            }
        }


        private SplitsService()
        {
            splitsManager = new();
        }

        public List<Splits?>? getAll()
        {
            return splitsManager.getAll()?.toListBO();
        }

        public List<Splits?>? getbyTransactionidNull()
        {
            return splitsManager.getbyTransactionidNull()?.toListBO();
        }

        public List<Splits?>? getbyTransactionid(int transactionid)
        {
            return splitsManager.getbyTransactionid(transactionid)?.toListBO();
        }

        public Splits? getByID(int? id)
        {
            return (Splits?)splitsManager.getByID(id);
        }

        public void update(Splits splits)
        {
            splitsManager.update(splits?.toDAO());
        }

        public void delete(Splits splits)
        {
            splitsManager.delete(splits?.toDAO());
        }

        public Decimal? getAmountTotal(Transactions transactions)
        {
            Decimal? total = 0;
            if (transactions.splits != null && transactions.splits.Count != 0)
            {
                foreach (Splits? splits in transactions.splits)
                {
                    total += splits.amountIn == null ? 0 : splits.amountIn;
                    total -= splits.amountOut == null ? 0 : splits.amountOut;
                }
            }

            return total;
        }

        public void saveChanges(Transactions? transactions, Splits splits)
        {
            if (splits.category == null && splits.categoryid != null)
            {
                splits.category = CategoriesService.Instance.getByID(splits.categoryid);
            }

            splits.amountIn ??= 0;

            splits.amountOut ??= 0;

            update(splits);
        }

        public int getNextID()
        {
            return splitsManager.getNextID();
        }
    }
}
