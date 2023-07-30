using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class TransactionsManager : ManagerBase<TransactionsDAO>
    {

#pragma warning disable CS8603
        protected override Expression<Func<TransactionsDAO, object>>[] GetIncludes()
        {
            return new Expression<Func<TransactionsDAO, object>>[]
            {
                a => a.account,
                a => a.person,
                a => a.category,
                a => a.tag,
                a => a.investmentProducts,
                a => a.transactionStatus
            };
        }
#pragma warning restore CS8603

        public IEnumerable<TransactionsDAO>? GetByAccount(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                var query = GetEntyWithInclude(repository)?
                    .Where(x => id.Equals(x.accountid));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<TransactionsDAO>? GetByAccountOrderByOrdenDesc(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                var query = GetEntyWithInclude(repository)?
                    .Where(x => id.Equals(x.accountid))
                    .OrderByDescending(x => x.orden);

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }
        public IEnumerable<TransactionsDAO>? GetByAccountOrderByOrdenDesc(int? id, int startIndex, int nPage)
        {
            return GetByAccountOrderByOrdenDesc(id)?.Skip(startIndex).Take(nPage);
        }

        public IEnumerable<TransactionsDAO>? GetAllOpenned()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                var query = GetEntyWithInclude(repository)?
                    .Where(x => !x.account.closed.HasValue || !x.account.closed.Value);

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<TransactionsDAO>? GetAllOpennedOrderByOrdenDesc(int startIndex, int nPage)
        {
            return GetAllOpennedOrderByOrdenDesc()?.Skip(startIndex).Take(nPage);
        }

        private IEnumerable<TransactionsDAO>? GetAllOpennedOrderByOrdenDesc()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                var query = GetEntyWithInclude(repository)?
                    .Where(x => !x.account.closed.HasValue || !x.account.closed.Value)
                    .OrderByDescending(x => x.orden);

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<TransactionsDAO>? GetByPerson(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                var query = GetEntyWithInclude(repository)?
                    .Where(x => id.Equals(x.personid));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<TransactionsDAO>? GetByInvestmentProduct(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                var query = GetEntyWithInclude(repository)?
                    .Where(x => id.Equals(x.investmentProductsid));

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public int GetNextId()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var cmd = unitOfWork.GetDataBase().
                    GetDbConnection().CreateCommand();
                cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'transactions';";

                unitOfWork.GetDataBase().OpenConnection();
                var result = cmd.ExecuteReader();
                result.Read();
                var id = Convert.ToInt32(result[0]);
                result.Close();

                return id;
            }
        }
    }
}
