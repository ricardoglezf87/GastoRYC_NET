using BBDDLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class TransactionsService
    {
        public List<Transactions>? getAll()
        {
            return RYCContextService.getInstance().BBDD.transactions?.ToList();
        }

        public Transactions? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.transactions?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(Transactions transactions)
        {            
            RYCContextService.getInstance().BBDD.Update(transactions);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(Transactions transactions)
        {
            RYCContextService.getInstance().BBDD.Remove(transactions);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }


        public int getNextID()
        {
            var cmd = RYCContextService.getInstance().BBDD.Database.
                GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT CASE WHEN (SELECT COUNT(1) FROM transactions) = 0 THEN 1 ELSE convert(int,IDENT_CURRENT('transactions') + 1) END AS Current_Identity;";
            
            RYCContextService.getInstance().BBDD.Database.OpenConnection();
            var result = cmd.ExecuteReader();
            result.Read();
            int id = (int) result[0];
            result.Close();

            return id;
        }

    }
}
