using DAOLib.Models;
using DAOLib.Repositories;

using System.Collections.Generic;

namespace DAOLib.Managers
{
    public class VBalancebyCategoryManager
    {
        public List<VBalancebyCategoryDAO>? getAll()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryGeneral<VBalancebyCategoryDAO>();
                return repository.GetAll();
            }
        }
    }
}
