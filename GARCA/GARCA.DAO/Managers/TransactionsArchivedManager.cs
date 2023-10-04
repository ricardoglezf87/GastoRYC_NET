using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class TransactionsArchivedManager : ManagerBase<TransactionsArchivedDao>
    {

#pragma warning disable CS8603
        protected override Expression<Func<TransactionsArchivedDao, object>>[] GetIncludes()
        {
            return new Expression<Func<TransactionsArchivedDao, object>>[]
            {
                a => a.Account,
                a => a.Person,
                a => a.Category,
                a => a.Tag,
                a => a.InvestmentProducts,
                a => a.TransactionStatus
            };
        }
#pragma warning restore CS8603

        public IEnumerable<TransactionsArchivedDao>? GetByAccount(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsArchivedDao>();
                var query = GetEntyWithInclude(repository)?
                    .Where(x => id.Equals(x.Accountid));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<TransactionsArchivedDao>? GetByPerson(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsArchivedDao>();
                var query = GetEntyWithInclude(repository)?
                    .Where(x => id.Equals(x.Personid));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<TransactionsArchivedDao>? GetByAccountOrderByOrdenDesc(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsArchivedDao>();
                var query = GetEntyWithInclude(repository)?
                    .Where(x => id.Equals(x.Accountid))
                    .OrderByDescending(x => x.Orden);

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }
        
        private IEnumerable<TransactionsArchivedDao>? GetAllOpennedOrderByOrdenDesc()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsArchivedDao>();
                var query = GetEntyWithInclude(repository)?
                    .Where(x => !x.Account.Closed.HasValue || !x.Account.Closed.Value)
                    .OrderByDescending(x => x.Orden);

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<TransactionsArchivedDao>? GetByInvestmentProduct(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsArchivedDao>();
                var query = GetEntyWithInclude(repository)?
                    .Where(x => id.Equals(x.InvestmentProductsid));

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
