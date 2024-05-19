using GARCA.wsData.Repositories;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class TransactionsStatusService : ServiceBase<TransactionsStatusRepository, TransactionsStatus>
    {
        public enum ETransactionsTypes
        {
            Pending = 1,
            Provisional = 2,
            Reconciled = 3
        }
    }
}
