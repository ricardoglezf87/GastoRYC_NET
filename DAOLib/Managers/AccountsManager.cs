using DAOLib.Models;
using DAOLib.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DAOLib.Managers
{
    public class AccountsManager : ManagerBase<AccountsDAO>
    {
#pragma warning disable CS8603
        public override Expression<Func<AccountsDAO, object>>[] getIncludes()
        {
            return new Expression<Func<AccountsDAO, object>>[]
            {
                a => a.accountsTypes,
                a => a.category
            };
        }
#pragma warning restore CS8603

        public List<AccountsDAO>? getAllOrderByAccountsTypesId()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<AccountsDAO>();
                return getEntyWithInclude(repository)?.OrderBy(x => x.accountsTypesid).ToList();
            }
        }

        public List<AccountsDAO>? getAllOpened()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<AccountsDAO>();
                return getEntyWithInclude(repository)?.Where(x => !x.closed.HasValue || !x.closed.Value).ToList();
            }
        }

        public AccountsDAO? getByCategoryId(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<AccountsDAO>();
                return repository?.entities?.FirstOrDefault(x => id.Equals(x.categoryid));
            }
        }
    }
}
