using GARCA.Data.Managers;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class SplitsRemindersService : ServiceBase<SplitsRemindersManager, SplitsReminders>
    {
        public async Task<IEnumerable<SplitsReminders>?> GetbyTransactionidNull()
        {
            return await manager.GetbyTransactionidNull();
        }

        public async Task<IEnumerable<SplitsReminders>?> GetbyTransactionid(int transactionid)
        {
            return await manager.GetbyTransactionid(transactionid);
        }

        public async Task Update(SplitsReminders splitsReminders)
        {
            await manager.Update(splitsReminders);
        }
    }
}
