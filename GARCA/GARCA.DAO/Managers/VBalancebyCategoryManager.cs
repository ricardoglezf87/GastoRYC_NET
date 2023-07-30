using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using System.Collections.Generic;

namespace GARCA.DAO.Managers
{
    public class VBalancebyCategoryManager
    {
        public IEnumerable<VBalancebyCategoryDAO>? GetAll()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryGeneral<VBalancebyCategoryDAO>();
                var query = repository.GetAll();

                foreach (var item in query)
                {
                    yield return item;
                }
            }
        }
    }
}
