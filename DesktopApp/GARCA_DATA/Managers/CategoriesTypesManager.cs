using GARCA.DAO.Repositories;
using GARCA.Models;

namespace GARCA.Data.Managers
{
    public class CategoriesTypesManager : ManagerBase<CategoriesTypes>
    {
        public enum ECategoriesTypes
        {
            Expenses = 1,
            Incomes = 2,
            Transfers = 3,
            Specials = 4
        }

        public IEnumerable<CategoriesTypes>? GetAllWithoutSpecialTransfer()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<CategoriesTypes>();
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
