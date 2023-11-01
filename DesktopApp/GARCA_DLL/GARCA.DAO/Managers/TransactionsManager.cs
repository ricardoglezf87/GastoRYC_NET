using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class TransactionsManager : ManagerBase<TransactionsDao>
    {

#pragma warning disable CS8603
        protected override Expression<Func<TransactionsDao, object>>[] GetIncludes()
        {
            return new Expression<Func<TransactionsDao, object>>[]
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

        public IEnumerable<TransactionsDao>? GetByAccount(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDao>();
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

        public IEnumerable<TransactionsDao>? GetByAccountOrderByOrdenDesc(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDao>();
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
        public IEnumerable<TransactionsDao>? GetByAccountOrderByOrdenDesc(int? id, int startIndex, int nPage)
        {
            return GetByAccountOrderByOrdenDesc(id)?.Skip(startIndex).Take(nPage);
        }

        public IEnumerable<TransactionsDao>? GetAllOpenned()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDao>();
                var query = GetEntyWithInclude(repository)?
                    .Where(x => !x.Account.Closed.HasValue || !x.Account.Closed.Value);

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<TransactionsDao>? GetAllOpennedOrderByOrdenDesc(int startIndex, int nPage)
        {
            return GetAllOpennedOrderByOrdenDesc()?.Skip(startIndex).Take(nPage);
        }

        private IEnumerable<TransactionsDao>? GetAllOpennedOrderByOrdenDesc()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDao>();
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

        public IEnumerable<TransactionsDao>? GetByPerson(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDao>();
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

        public IEnumerable<TransactionsDao>? GetByInvestmentProduct(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsDao>();
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
