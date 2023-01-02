using BBDDLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace GastosRYC.BBDDLib.Services
{
    public class TransactionsStatusService : ITransactionsStatusService
    {
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
