using BOLib.Extensions;
using BOLib.Helpers;
using BOLib.Models;
using DAOLib.Managers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
{
    public class SplitsService
    {

        private readonly SplitsManager splitsManager;
        private readonly TransactionsService transactionsService;
        private readonly CategoriesService categoriesService;


        public SplitsService()
        {
            splitsManager = new();
            transactionsService = new();
            categoriesService = new();
        }

        public List<Splits>? getAll()
        {
            return splitsManager.getAll()?.toListBO();
        }

        public List<Splits>? getbyTransactionidNull()
        {
            return splitsManager.getbyTransactionidNull()?.toListBO();
        }

        public List<Splits>? getbyTransactionid(int transactionid)
        {
            return splitsManager.getbyTransactionid(transactionid)?.toListBO();
        }

        public Splits? getByID(int? id)
        {
            return (Splits)splitsManager.getByID(id);
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
                foreach (Splits splits in transactions.splits)
                {
                    total += (splits.amountIn == null ? 0 : splits.amountIn);
                    total -= (splits.amountOut == null ? 0 : splits.amountOut);
                }
            }

            return total;
        }

        public void saveChanges(Transactions? transactions, Splits splits)
        {
            if (splits.category == null && splits.categoryid != null)
            {
                splits.category = categoriesService.getByID(splits.categoryid);
            }

            if (splits.amountIn == null)
                splits.amountIn = 0;

            if (splits.amountOut == null)
                splits.amountOut = 0;

            update(splits);
        }

        public int getNextID()
        {
            var cmd = RYCContextService.getInstance().BBDD.Database.
                GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'splits';";

            RYCContextService.getInstance().BBDD.Database.OpenConnection();
            var result = cmd.ExecuteReader();
            result.Read();
            int id = Convert.ToInt32(result[0]);
            result.Close();

            return id;
        }
    }
}
