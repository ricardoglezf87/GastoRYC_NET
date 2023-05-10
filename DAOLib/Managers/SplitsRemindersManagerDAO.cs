using DAOLib.Models;
using DAOLib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Managers
{
    public class SplitsRemindersManagerDAO : IManagerDAO<SplitsRemindersDAO>
    {

        public List<SplitsRemindersDAO>? getbyTransactionidNull()
        {
            return RYCContextServiceDAO.getInstance().BBDD.splitsReminders?.Where(x => x.transactionid == null).ToList();
        }

        public List<SplitsRemindersDAO>? getbyTransactionid(int transactionid)
        {
            return RYCContextServiceDAO.getInstance().BBDD.splitsReminders?.Where(x => x.transactionid == transactionid).ToList();
        }

        public Decimal? getAmountTotal(TransactionsRemindersDAO transactions)
        {
            Decimal? total = 0;
            if (transactions.splits != null && transactions.splits.Count != 0)
            {
                foreach (SplitsRemindersDAO splitsReminders in transactions.splits)
                {
                    total += (splitsReminders.amountIn == null ? 0 : splitsReminders.amountIn);
                    total -= (splitsReminders.amountOut == null ? 0 : splitsReminders.amountOut);
                }
            }

            return total;
        }

        public int getNextID()
        {
            var cmd = RYCContextServiceDAO.getInstance().BBDD.Database.
                GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'splitsReminders';";

            RYCContextServiceDAO.getInstance().BBDD.Database.OpenConnection();
            var result = cmd.ExecuteReader();
            result.Read();
            int id = Convert.ToInt32(result[0]);
            result.Close();

            return id;
        }
    }
}
