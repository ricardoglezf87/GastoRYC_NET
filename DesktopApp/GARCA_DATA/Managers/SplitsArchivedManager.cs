using GARCA.DAO.Repositories;
using GARCA.Models;
using System.Linq.Expressions;

namespace GARCA.Data.Managers
{
    public class SplitsArchivedManager : ManagerBase<SplitsArchived>
    {
#pragma warning disable CS8603
        protected override Expression<Func<SplitsArchived, object>>[] GetIncludes()
        {
            return new Expression<Func<SplitsArchived, object>>[]
            {
                a => a.Transaction,
                a => a.Category,
                a => a.Tag
            };
        }
#pragma warning restore CS8603

        public IEnumerable<SplitsArchived>? GetbyTransactionid(int transactionid)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsArchived>();
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
