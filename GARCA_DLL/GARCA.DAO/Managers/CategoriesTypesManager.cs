using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using System.Collections.Generic;
using System.Linq;

namespace GARCA.DAO.Managers
{
    public class CategoriesTypesManager : ManagerBase<CategoriesTypesDao>
    {
        public enum ECategoriesTypes
        {
            Expenses = 1,
            Incomes = 2,
            Transfers = 3,
            Specials = 4
        }

        public IEnumerable<CategoriesTypesDao>? GetAllWithoutSpecialTransfer()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<CategoriesTypesDao>();
                var query = repository.GetAll()
                    .Where(x => x.Id is not (int)ECategoriesTypes.Specials and
                    not (int)ECategoriesTypes.Transfers);

                foreach (var item in query)
                {
                    yield return item;
                }
            }
        }
    }
}
