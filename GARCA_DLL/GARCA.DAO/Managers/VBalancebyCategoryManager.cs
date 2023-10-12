using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

namespace GARCA.DAO.Managers
{
    public class VBalancebyCategoryManager
    {
        public IEnumerable<VBalancebyCategoryDao>? GetAll()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryGeneral<VBalancebyCategoryDao>();
                var query = repository.GetAll();

                foreach (var item in query)
                {
                    yield return item;
                }
            }
        }
    }
}
