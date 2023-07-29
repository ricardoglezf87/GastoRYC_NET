using GARCA.BO.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System;
using System.Collections.Generic;
using GARCA.IOC;

namespace GARCA.BO.Services
{
    public class SplitsService
    {

        private readonly SplitsManager splitsManager;
        
        public SplitsService()
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
                splits.category = DependencyConfig.iCategoriesService.getByID(splits.categoryid);
            }

            splits.amountIn ??= 0;

            splits.amountOut ??= 0;

            update(splits);
        }

    }
}
