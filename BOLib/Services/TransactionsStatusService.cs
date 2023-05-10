using BOLib.Helpers;
using BOLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
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
            return MapperConfig.InitializeAutomapper().Map<List<TransactionsStatus>>(RYCContextService.getInstance().BBDD.transactionsStatus?.ToList());
        }

        public TransactionsStatus? getFirst()
        {
            return MapperConfig.InitializeAutomapper().Map<TransactionsStatus>(RYCContextService.getInstance().BBDD.transactionsStatus?.FirstOrDefault());
        }

        public TransactionsStatus? getByID(int? id)
        {
            return MapperConfig.InitializeAutomapper().Map<TransactionsStatus>(RYCContextService.getInstance().BBDD.transactionsStatus?.FirstOrDefault(x => id.Equals(x.id)));
        }
    }
}
