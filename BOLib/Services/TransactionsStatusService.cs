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
            return transactionsStatusManager.getAll()?.toListTransactionsStatus();
        }

        public TransactionsStatus? getFirst()
        {
            return MapperConfig.InitializeAutomapper().Map<TransactionsStatus>(RYCContextService.getInstance().BBDD.transactionsStatus?.FirstOrDefault());
        }

        public TransactionsStatus? getByID(int? id)
        {
            return (TransactionsStatus)transactionsStatusManager.getByID(id);
        }
    }
}
