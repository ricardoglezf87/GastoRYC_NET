using DAOLib.Models;
using DAOLib.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DAOLib.Managers
{
    public class SplitsRemindersManager : ManagerBase<SplitsRemindersDAO>
    {
#pragma warning disable CS8603
        public override Expression<Func<SplitsRemindersDAO, object>>[] getIncludes()
        {
            return new Expression<Func<SplitsRemindersDAO, object>>[]
            {
                a => a.transaction,
                a => a.category,
                a => a.tag
            };
        }
#pragma warning restore CS8603

        public List<SplitsRemindersDAO>? getbyTransactionidNull()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsRemindersDAO>();
                return getEntyWithInclude(repository)?.Where(x => x.transactionid == null).ToList();
            }
        }

        public List<SplitsRemindersDAO>? getbyTransactionid(int transactionid)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsRemindersDAO>();
                return getEntyWithInclude(repository)?.Where(x => x.transactionid == transactionid).ToList();
            }
        }

        public Decimal? getAmountTotal(TransactionsRemindersDAO transactions)
        {
            Decimal? total = 0;
            if (transactions.splits != null && transactions.splits.Count != 0)
            {
                foreach (SplitsRemindersDAO splitsReminders in transactions.splits)
                {
                    total += splitsReminders.amountIn == null ? 0 : splitsReminders.amountIn;
                    total -= splitsReminders.amountOut == null ? 0 : splitsReminders.amountOut;
                }
            }

            return total;
        }

        public int getNextID()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var cmd = unitOfWork.getDataBase().
                GetDbConnection().CreateCommand();
                cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'splitsReminders';";

                unitOfWork.getDataBase().OpenConnection();
                var result = cmd.ExecuteReader();
                result.Read();
                int id = Convert.ToInt32(result[0]);
                result.Close();

                return id;
            }
        }
    }
}
