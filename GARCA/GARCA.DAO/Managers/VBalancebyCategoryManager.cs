using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using System.Collections.Generic;

namespace GARCA.DAO.Managers
{
    public class VBalancebyCategoryManager
    {
        public HashSet<VBalancebyCategoryDAO>? getAll()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryGeneral<VBalancebyCategoryDAO>();
                return repository.GetAll();
            }
        }
    }
}
