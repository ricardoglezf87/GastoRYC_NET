using GARCA.BO.Models;
using GARCA.DAO.Managers;
using GARCA.Utlis.Extensions;

namespace GARCA.BO.Services
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
            return transactionsStatusManager.GetAll()?.ToHashSetBo();
        }

        public TransactionsStatus? GetById(int? id)
        {
            return (TransactionsStatus?)transactionsStatusManager.GetById(id);
        }
    }
}
