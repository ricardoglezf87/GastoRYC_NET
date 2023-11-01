using GARCA.Models;
using GARCA.DAO.Repositories;

namespace GARCA.Data.Managers
{
    public class VBalancebyCategoryManager
    {
        public IEnumerable<VBalancebyCategory>? GetAll()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryGeneral<VBalancebyCategory>();
                var query = repository.GetAll();

                foreach (var item in query)
                {
                    yield return item;
                }
            }
        }
    }
}
