using GARCA.BO.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class SplitsService
    {

        private readonly SplitsManager splitsManager;
        private static SplitsService? _instance;
        private static readonly object _lock = new();

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

    }
}
