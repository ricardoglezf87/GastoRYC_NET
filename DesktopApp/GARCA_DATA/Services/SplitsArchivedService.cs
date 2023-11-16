using GARCA.Data.Managers;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class SplitsArchivedService : ServiceBase<SplitsArchivedManager, SplitsArchived, Int32>
    {
        public async Task<IEnumerable<SplitsArchived>?> GetbyTransactionid(int transactionid)
        {
            return await manager.GetbyTransactionid(transactionid);
        }        
    }
}
