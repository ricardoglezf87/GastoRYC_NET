using BOLib.Extensions;
using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;

namespace BOLib.Services
{
    public class TransactionsStatusService
    {
        private readonly TransactionsStatusManager transactionsStatusManager;
        private static TransactionsStatusService? _instance;
        private static readonly object _lock = new object();

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

        public List<TransactionsStatus?>? getAll()
        {
            return transactionsStatusManager.getAll()?.toListBO();
        }

        public TransactionsStatus? getFirst()
        {
            return (TransactionsStatus?)transactionsStatusManager.getFirst();
        }

        public TransactionsStatus? getByID(int? id)
        {
            return (TransactionsStatus?)transactionsStatusManager.getByID(id);
        }
    }
}
