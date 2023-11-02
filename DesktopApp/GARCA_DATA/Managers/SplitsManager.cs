using GARCA.DAO.Repositories;
using GARCA.Models;
using System.Linq.Expressions;

namespace GARCA.Data.Managers
{
    public class SplitsManager : ManagerBase<Splits>
    {
#pragma warning disable CS8603
        protected override Expression<Func<Splits, object>>[] GetIncludes()
        {
            return new Expression<Func<Splits, object>>[]
            {
                a => a.Transaction,
                a => a.Category,
                a => a.Tag
            };
        }
#pragma warning restore CS8603

        public IEnumerable<Splits>? GetbyTransactionidNull()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<Splits>();
                var query = GetEntyWithInclude(repository)?.Where(x => x.Transactionid == null);

                if (query != null)
                {
                    foreach (var item in query)
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<Splits>? GetbyTransactionid(int transactionid)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<Splits>();
                var query = GetEntyWithInclude(repository)?.Where(x => x.Transactionid == transactionid);

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
