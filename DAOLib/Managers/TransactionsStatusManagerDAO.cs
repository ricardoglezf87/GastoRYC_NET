using DAOLib.Models;
using DAOLib.Services;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Managers
{
    public class TransactionsStatusManagerDAO : IManagerDAO<TransactionsStatusDAO>
    {
        public enum eTransactionsTypes : int
        {
            Pending = 1,
            Provisional = 2,
            Reconciled = 3
        }

        public TransactionsStatusDAO? getFirst()
        {
            return RYCContextServiceDAO.getInstance().BBDD.transactionsStatus?.FirstOrDefault();
        }
    }
}
