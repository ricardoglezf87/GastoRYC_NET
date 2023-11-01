using GARCA.Models;
using GARCA.DAO.Repositories;
using System.Linq.Expressions;

namespace GARCA.Data.Managers
{
    public class AccountsManager : ManagerBase<Accounts>
    {
#pragma warning disable CS8603
        protected override Expression<Func<Accounts, object>>[] GetIncludes()
        {
            return new Expression<Func<Accounts, object>>[]
            {
                a => a.AccountsTypes,
                a => a.Category
            };
        }
#pragma warning restore CS8603

        public IEnumerable<Accounts>? GetAllOpened()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<Accounts>();
                var query = GetEntyWithInclude(repository)?.Where(x => !x.Closed.HasValue || !x.Closed.Value);

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public Accounts? GetByCategoryId(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<Accounts>();
                return repository.Entities.FirstOrDefault(x => id.Equals(x.Categoryid));
            }
        }
    }
}
