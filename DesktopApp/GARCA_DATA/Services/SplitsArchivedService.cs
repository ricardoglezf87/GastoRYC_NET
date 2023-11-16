using GARCA.Data.Managers;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class SplitsArchivedService : ServiceBase<SplitsArchivedManager, SplitsArchived>
    {
        public async Task<IEnumerable<SplitsArchived>?> GetbyTransactionid(int transactionid)
        {
            return await manager.GetbyTransactionid(transactionid);
        }        
    }
}
