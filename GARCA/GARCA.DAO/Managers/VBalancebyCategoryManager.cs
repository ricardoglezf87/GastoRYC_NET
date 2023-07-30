using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
