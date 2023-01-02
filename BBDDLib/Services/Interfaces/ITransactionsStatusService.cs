using BBDDLib.Models;
using System.Collections.Generic;

namespace GastosRYC.BBDDLib.Services
{
    public interface ITransactionsStatusService
    {
        public enum eTransactionsTypes : int
        {
            Pending = 1,
            Provisional = 2,
            Reconciled = 3
        }

        public List<TransactionsStatus>? getAll();

        public TransactionsStatus? getFirst();

        public TransactionsStatus? getByID(int? id);
    }
}
