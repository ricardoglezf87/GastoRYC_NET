using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using System.Linq;

namespace GARCA.DAO.Managers
{
    public class TransactionsStatusManager : ManagerBase<TransactionsStatusDAO>
    {
        public TransactionsStatusDAO? getFirst()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsStatusDAO>();
                return getEntyWithInclude(repository)?.FirstOrDefault();
            }
        }
    }
}
