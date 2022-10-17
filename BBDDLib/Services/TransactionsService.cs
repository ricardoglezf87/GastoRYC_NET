using BBDDLib.Models;
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
            return RYCContextService.Instance.BBDD.transactions?.ToList();
        }

        public Transactions? getByID(int? id)
        {
            return RYCContextService.Instance.BBDD.transactions?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(Transactions transactions)
        {
            RYCContextService.Instance.BBDD.Update(transactions);
            RYCContextService.Instance.BBDD.SaveChanges();
        }
    }
}
