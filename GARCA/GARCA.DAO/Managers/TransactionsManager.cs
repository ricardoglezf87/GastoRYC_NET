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
        public override Expression<Func<TransactionsDAO, object>>[] getIncludes()
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

        public IEnumerable<TransactionsDAO>? getByAccount(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                var query = getEntyWithInclude(repository)?
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

        public IEnumerable<TransactionsDAO>? getByAccount(int? id, int startIndex, int nPage)
        {
            return getByAccount(id)?.Skip(startIndex)?.Take(nPage)?.ToList();
        }

        public IEnumerable<TransactionsDAO>? getByAccountOrderByOrdenDesc(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                var query = getEntyWithInclude(repository)?
                    .Where(x => id.Equals(x.accountid))?
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
        public IEnumerable<TransactionsDAO>? getByAccountOrderByOrdenDesc(int? id, int startIndex, int nPage)
        {
            return getByAccountOrderByOrdenDesc(id)?.Skip(startIndex)?.Take(nPage)?.ToList();
        }

        public IEnumerable<TransactionsDAO>? getAllOpenned()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                var query = getEntyWithInclude(repository)?
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

        public IEnumerable<TransactionsDAO>? getAllOpennedOrderByOrdenDesc(int startIndex, int nPage)
        {
            return getAllOpennedOrderByOrdenDesc()?.Skip(startIndex)?.Take(nPage)?.ToList();
        }

        private IEnumerable<TransactionsDAO>? getAllOpennedOrderByOrdenDesc()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                var query = getEntyWithInclude(repository)?
                    .Where(x => !x.account.closed.HasValue || !x.account.closed.Value)?
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

        public IEnumerable<TransactionsDAO>? getByPerson(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                var query = getEntyWithInclude(repository)?
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
        
        public IEnumerable<TransactionsDAO>? getByInvestmentProduct(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDAO>();
                var query = getEntyWithInclude(repository)?
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

        public int getNextID()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var cmd = unitOfWork.getDataBase().
                    GetDbConnection().CreateCommand();
                cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'transactions';";

                unitOfWork.getDataBase().OpenConnection();
                var result = cmd.ExecuteReader();
                result.Read();
                var id = Convert.ToInt32(result[0]);
                result.Close();

                return id;
            }
        }
    }
}
