using GARCA.BO.Extensions;
using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class TransactionsStatusService
    {
        private readonly TransactionsStatusManager transactionsStatusManager;
        private static TransactionsStatusService? _instance;
        private static readonly object _lock = new();

        public static TransactionsStatusService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new TransactionsStatusService();
                    }
                }
                return _instance;
            }
        }

        private TransactionsStatusService()
        {
            transactionsStatusManager = new();
        }

        public enum eTransactionsTypes : int
        {
            Pending = 1,
            Provisional = 2,
            Reconciled = 3
        }

        public HashSet<TransactionsStatus?>? getAll()
        {
            return transactionsStatusManager.getAll()?.toHashSetBO();
        }

        public TransactionsStatus? getByID(int? id)
        {
            return (TransactionsStatus?)transactionsStatusManager.getByID(id);
        }
    }
}
