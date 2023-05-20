using DAOLib.Models;
using DAOLib.Repositories;

using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Managers
{
    public class CategoriesTypesManager : ManagerBase<CategoriesTypesDAO>
    {
        public enum eCategoriesTypes : int
        {
            Expenses = 1,
            Incomes = 2,
            Transfers = 3,
            Specials = 4
        }

        public List<CategoriesTypesDAO>? getAllFilterTransfer()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<CategoriesTypesDAO>();
                return repository.GetAll()?
                    .Where(x => !x.id.Equals((int)eCategoriesTypes.Transfers) &&
                    !x.id.Equals((int)eCategoriesTypes.Transfers)).ToList();
            }
        }
    }
}
