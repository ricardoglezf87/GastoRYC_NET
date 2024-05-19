using GARCA.wsData.Repositories;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class SplitsRemindersService : ServiceBase<SplitsRemindersRepository, SplitsReminders>
    {
        public async Task<IEnumerable<SplitsReminders>?> GetbyTransactionidNull()
        {
            return await repository.GetbyTransactionidNull();
        }

        public async Task<IEnumerable<SplitsReminders>?> GetbyTransactionid(int transactionid)
        {
            return await repository.GetbyTransactionid(transactionid);
        }
    }
}
