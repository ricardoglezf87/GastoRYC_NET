using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class AccountsManager : ManagerBase<AccountsDAO>
    {
#pragma warning disable CS8603
        public override Expression<Func<AccountsDAO, object>>[] GetIncludes()
        {
            return new Expression<Func<AccountsDAO, object>>[]
            {
                a => a.accountsTypes,
                a => a.category
            };
        }
#pragma warning restore CS8603

        public IEnumerable<AccountsDAO>? GetAllOpened()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<AccountsDAO>();
                var query = GetEntyWithInclude(repository)?.Where(x => !x.closed.HasValue || !x.closed.Value);

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public AccountsDAO? GetByCategoryId(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<AccountsDAO>();
                return repository?.Entities?.FirstOrDefault(x => id.Equals(x.categoryid));
            }
        }
    }
}
