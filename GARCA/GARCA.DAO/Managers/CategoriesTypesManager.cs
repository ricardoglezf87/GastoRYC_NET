using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using System.Collections.Generic;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GARCA.DAO.Managers
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

        public IEnumerable<CategoriesTypesDAO>? getAllWithoutSpecialTransfer()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<CategoriesTypesDAO>();
                var query = repository.GetAll()?
                    .Where(x => x.id is not (int)eCategoriesTypes.Specials and
                    not (int)eCategoriesTypes.Transfers);
                
                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
