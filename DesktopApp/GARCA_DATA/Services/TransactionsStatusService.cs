using GARCA.Models;
using GARCA.Data.Managers;


namespace GARCA.Data.Services
{
    public class TransactionsStatusService
    {
        private readonly TransactionsStatusManager transactionsStatusManager;

        public TransactionsStatusService()
        {
            transactionsStatusManager = new TransactionsStatusManager();
        }

        public enum ETransactionsTypes
        {
            Pending = 1,
            Provisional = 2,
            Reconciled = 3
        }

        public HashSet<TransactionsStatus?>? GetAll()
        {
            return transactionsStatusManager.GetAll()?.ToHashSet();
        }

        public TransactionsStatus? GetById(int? id)
        {
            return (TransactionsStatus)transactionsStatusManager.GetById(id);
        }
    }
}
