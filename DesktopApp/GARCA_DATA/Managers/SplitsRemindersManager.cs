using GARCA.Models;
using GARCA.DAO.Repositories;
using System.Linq.Expressions;

namespace GARCA.Data.Managers
{
    public class SplitsRemindersManager : ManagerBase<SplitsReminders>
    {
#pragma warning disable CS8603
        protected override Expression<Func<SplitsReminders, object>>[] GetIncludes()
        {
            return new Expression<Func<SplitsReminders, object>>[]
            {
                a => a.Transaction,
                a => a.Category,
                a => a.Tag
            };
        }
#pragma warning restore CS8603

        public IEnumerable<SplitsReminders>? GetbyTransactionidNull()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsReminders>();
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

        public IEnumerable<SplitsReminders>? GetbyTransactionid(int transactionid)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsReminders>();
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
