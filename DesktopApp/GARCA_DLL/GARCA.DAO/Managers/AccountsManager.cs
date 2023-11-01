using GARCA.DAO.Models;
using GARCA.DAO.Repositories;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class AccountsManager : ManagerBase<AccountsDao>
    {
#pragma warning disable CS8603
        protected override Expression<Func<AccountsDao, object>>[] GetIncludes()
        {
            return new Expression<Func<AccountsDao, object>>[]
            {
                a => a.AccountsTypes,
                a => a.Category
            };
        }
#pragma warning restore CS8603

        public IEnumerable<AccountsDao>? GetAllOpened()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<AccountsDao>();
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

        public AccountsDao? GetByCategoryId(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<AccountsDao>();
                return repository.Entities.FirstOrDefault(x => id.Equals(x.Categoryid));
            }
        }
    }
}
