using DAOLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Services
{
    public class TransactionsStatusServiceDAO
    {
        public enum eTransactionsTypes : int
        {
            Pending = 1,
            Provisional = 2,
            Reconciled = 3
        }

        public List<TransactionsStatusDAO>? getAll()
        {
            return RYCContextServiceDAO.getInstance().BBDD.transactionsStatus?.ToList();
        }

        public TransactionsStatusDAO? getFirst()
        {
            return RYCContextServiceDAO.getInstance().BBDD.transactionsStatus?.FirstOrDefault();
        }

        public TransactionsStatusDAO? getByID(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.transactionsStatus?.FirstOrDefault(x => id.Equals(x.id));
        }
    }
}
