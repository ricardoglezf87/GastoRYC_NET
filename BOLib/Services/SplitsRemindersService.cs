using BOLib.Helpers;
using BOLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
{
    public class SplitsRemindersService
    {
        public List<SplitsReminders>? getAll()
        {
            return MapperConfig.InitializeAutomapper().Map<List<SplitsReminders>>(RYCContextService.getInstance().BBDD.splitsReminders?.ToList());
        }

        public List<SplitsReminders>? getbyTransactionidNull()
        {
            return MapperConfig.InitializeAutomapper().Map<List<SplitsReminders>>(RYCContextService.getInstance().BBDD.splitsReminders?.Where(x => x.transactionid == null).ToList());
        }

        public List<SplitsReminders>? getbyTransactionid(int transactionid)
        {
            return MapperConfig.InitializeAutomapper().Map<List<SplitsReminders>>(RYCContextService.getInstance().BBDD.splitsReminders?.Where(x => x.transactionid == transactionid).ToList());
        }

        public SplitsReminders? getByID(int? id)
        {
            return MapperConfig.InitializeAutomapper().Map<SplitsReminders>(RYCContextService.getInstance().BBDD.splitsReminders?.FirstOrDefault(x => id.Equals(x.id)));
        }

        public void update(SplitsReminders splitsReminders)
        {
            RYCContextService.getInstance().BBDD.Update(splitsReminders);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(SplitsReminders splitsReminders)
        {
            RYCContextService.getInstance().BBDD.Remove(splitsReminders);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public Decimal? getAmountTotal(TransactionsReminders transactions)
        {
            Decimal? total = 0;
            if (transactions.splits != null && transactions.splits.Count != 0)
            {
                foreach (SplitsReminders splitsReminders in transactions.splits)
                {
                    total += (splitsReminders.amountIn == null ? 0 : splitsReminders.amountIn);
                    total -= (splitsReminders.amountOut == null ? 0 : splitsReminders.amountOut);
                }
            }

            return total;
        }

        public int getNextID()
        {
            var cmd = RYCContextService.getInstance().BBDD.Database.
                GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'splitsReminders';";

            RYCContextService.getInstance().BBDD.Database.OpenConnection();
            var result = cmd.ExecuteReader();
            result.Read();
            int id = Convert.ToInt32(result[0]);
            result.Close();

            return id;
        }
    }
}
