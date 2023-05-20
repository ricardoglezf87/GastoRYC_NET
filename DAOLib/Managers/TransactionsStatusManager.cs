using DAOLib.Models;
using DAOLib.Repositories;

using System.Linq;

namespace DAOLib.Managers
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
