using BBDDLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class SplitsReminderService
    {
        public List<SplitsReminder>? getAll()
        {
            return RYCContextService.getInstance().BBDD.splitsReminder?.ToList();
        }

        public List<SplitsReminder>? getbyTransactionidNull()
        {
            return RYCContextService.getInstance().BBDD.splitsReminder?.Where(x => x.transactionid == null).ToList();
        }

        public List<SplitsReminder>? getbyTransactionid(int transactionid)
        {
            return RYCContextService.getInstance().BBDD.splitsReminder?.Where(x=>x.transactionid == transactionid).ToList();
        }

        public SplitsReminder? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.splitsReminder?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(SplitsReminder splitsReminder)
        {
            RYCContextService.getInstance().BBDD.Update(splitsReminder);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(SplitsReminder splitsReminder)
        {
            RYCContextService.getInstance().BBDD.Remove(splitsReminder);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public Decimal? getAmountTotal(TransactionsReminder transactions)
        {
            Decimal? total = 0;
            if (transactions.splits != null && transactions.splits.Count != 0)
            {
                foreach (SplitsReminder splitsReminder in transactions.splits)
                {
                    total += (splitsReminder.amountIn == null ? 0 : splitsReminder.amountIn);
                    total -= (splitsReminder.amountOut == null ? 0 : splitsReminder.amountOut);
                }
            }

            return total;
        }

        public int getNextID()
        {
            var cmd = RYCContextService.getInstance().BBDD.Database.
                GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'splitsReminder';";

            RYCContextService.getInstance().BBDD.Database.OpenConnection();
            var result = cmd.ExecuteReader();
            result.Read();
            int id = Convert.ToInt32(result[0]);
            result.Close();

            return id;
        }
    }
}
