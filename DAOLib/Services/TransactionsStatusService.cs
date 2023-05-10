using DAOLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Services
{
    public class TransactionsStatusService
    {
        public enum eTransactionsTypes : int
        {
            Pending = 1,
            Provisional = 2,
            Reconciled = 3
        }

        public List<TransactionsStatus>? getAll()
        {
            return RYCContextService.getInstance().BBDD.transactionsStatus?.ToList();
        }

        public TransactionsStatus? getFirst()
        {
            return RYCContextService.getInstance().BBDD.transactionsStatus?.FirstOrDefault();
        }

        public TransactionsStatus? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.transactionsStatus?.FirstOrDefault(x => id.Equals(x.id));
        }
    }
}
