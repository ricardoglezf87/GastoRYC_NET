using GARCA.DAO.Models;
using GARCA.DAO.Repositories;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class SplitsArchivedManager : ManagerBase<SplitsArchivedDao>
    {
#pragma warning disable CS8603
        protected override Expression<Func<SplitsArchivedDao, object>>[] GetIncludes()
        {
            return new Expression<Func<SplitsArchivedDao, object>>[]
            {
                a => a.Transaction,
                a => a.Category,
                a => a.Tag
            };
        }
#pragma warning restore CS8603

        public IEnumerable<SplitsArchivedDao>? GetbyTransactionid(int transactionid)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsArchivedDao>();
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
