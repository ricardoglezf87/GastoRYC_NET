using DAOLib.Models;
using DAOLib.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Managers
{
    public class InvestmentProductsManager : ManagerBase<InvestmentProductsDAO>
    {
        public List<InvestmentProductsDAO>? getAllOpened()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<InvestmentProductsDAO>();
                return getAll()?.Where(x => x.active.HasValue && x.active.Value).ToList();
            }
        }
    }
}
