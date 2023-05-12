using BOLib.Extensions;
using BOLib.Helpers;
using BOLib.Models;
using DAOLib.Managers;
using DAOLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
{
    public class TransactionsStatusService
    {
        private readonly TransactionsStatusManager transactionsStatusManager;

        public TransactionsStatusService()
        {
            transactionsStatusManager = new();
        }

        public enum eTransactionsTypes : int
        {
            Pending = 1,
            Provisional = 2,
            Reconciled = 3
        }

        public List<TransactionsStatus>? getAll()
        {
            return transactionsStatusManager.getAll()?.toListBO();
        }

        public TransactionsStatus? getFirst()
        {
            return (TransactionsStatus)transactionsStatusManager.getFirst();
        }

        public TransactionsStatus? getByID(int? id)
        {
            return (TransactionsStatus)transactionsStatusManager.getByID(id);
        }
    }
}
