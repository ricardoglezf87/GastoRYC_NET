using DAOLib.Models;
using DAOLib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Managers
{
    public class SplitsManager : ManagerBase<SplitsDAO>
    {

        private readonly SimpleInjector.Container servicesContainer;

        public SplitsManager(SimpleInjector.Container servicesContainer)
        {
            this.servicesContainer = servicesContainer;
        }
  
        public List<SplitsDAO>? getbyTransactionidNull()
        {
            return RYCContextServiceDAO.getInstance().BBDD.splits?.Where(x => x.transactionid == null).ToList();
        }

        public List<SplitsDAO>? getbyTransactionid(int transactionid)
        {
            return RYCContextServiceDAO.getInstance().BBDD.splits?.Where(x => x.transactionid == transactionid).ToList();
        }

        public Decimal? getAmountTotal(TransactionsDAO transactions)
        {
            Decimal? total = 0;
            if (transactions.splits != null && transactions.splits.Count != 0)
            {
                foreach (SplitsDAO splits in transactions.splits)
                {
                    total += (splits.amountIn == null ? 0 : splits.amountIn);
                    total -= (splits.amountOut == null ? 0 : splits.amountOut);
                }
            }

            return total;
        }

        public void saveChanges(TransactionsDAO? transactions, SplitsDAO splits)
        {
            if (splits.category == null && splits.categoryid != null)
            {
                splits.category = servicesContainer.GetInstance<CategoriesManager>().getByID(splits.categoryid);
            }

            if (splits.amountIn == null)
                splits.amountIn = 0;

            if (splits.amountOut == null)
                splits.amountOut = 0;

            update(splits);
        }

        public int getNextID()
        {
            var cmd = RYCContextServiceDAO.getInstance().BBDD.Database.
                GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'splits';";

            RYCContextServiceDAO.getInstance().BBDD.Database.OpenConnection();
            var result = cmd.ExecuteReader();
            result.Read();
            int id = Convert.ToInt32(result[0]);
            result.Close();

            return id;
        }
    }
}
