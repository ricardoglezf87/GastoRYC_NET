using GARCA.Data.Managers;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class TransactionsStatusService : ServiceBase<TransactionsStatusManager, TransactionsStatus>
    {
        public enum ETransactionsTypes
        {
            Pending = 1,
            Provisional = 2,
            Reconciled = 3
        }
    }
}
