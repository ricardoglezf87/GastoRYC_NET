using GARCA.DAO.Repositories;
using GARCA.Models;
using System.Linq.Expressions;

namespace GARCA.Data.Managers
{
    public class TransactionsArchivedManager : ManagerBase<TransactionsArchived>
    {

#pragma warning disable CS8603
        protected override Expression<Func<TransactionsArchived, object>>[] GetIncludes()
        {
            return new Expression<Func<TransactionsArchived, object>>[]
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

        public IEnumerable<TransactionsArchived>? GetByAccount(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsArchived>();
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

        public IEnumerable<TransactionsArchived>? GetByPerson(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsArchived>();
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

        public IEnumerable<TransactionsArchived>? GetByAccountOrderByOrdenDesc(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsArchived>();
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

        private IEnumerable<TransactionsArchived>? GetAllOpennedOrderByOrdenDesc()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsArchived>();
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

        public IEnumerable<TransactionsArchived>? GetByInvestmentProduct(int? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<TransactionsArchived>();
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
