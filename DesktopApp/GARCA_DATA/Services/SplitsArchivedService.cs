using GARCA.Data.Managers;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class SplitsArchivedService : ServiceBase<SplitsArchivedManager, SplitsArchived, Int32>
    {
        public HashSet<SplitsArchived>? GetbyTransactionid(int transactionid)
        {
            return manager.GetbyTransactionid(transactionid)?.ToHashSet();
        }

        public void Update(SplitsArchived splits)
        {
            manager.Update(splits);
        }
    }
}
