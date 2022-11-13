using BBDDLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class SplitsService
    {
        public List<Splits>? getAll()
        {
            return RYCContextService.getInstance().BBDD.splits?.ToList();
        }

        public List<Splits>? getbyTransactionid(int transactionid)
        {
            return RYCContextService.getInstance().BBDD.splits?.Where(x=>x.transactionid == transactionid).ToList();
        }

        public Splits? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.splits?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(Splits splits)
        {
            RYCContextService.getInstance().BBDD.Update(splits);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(Splits splits)
        {
            RYCContextService.getInstance().BBDD.Remove(splits);
            RYCContextService.getInstance().BBDD.SaveChanges();
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
