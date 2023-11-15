using GARCA.Data.Managers;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class SplitsRemindersService : ServiceBase<SplitsRemindersManager, SplitsReminders, Int32>
    {
        public HashSet<SplitsReminders>? GetbyTransactionidNull()
        {
            return manager.GetbyTransactionidNull()?.ToHashSet();
        }

        public HashSet<SplitsReminders>? GetbyTransactionid(int transactionid)
        {
            return manager.GetbyTransactionid(transactionid)?.ToHashSet();
        }

        public void Update(SplitsReminders splitsReminders)
        {
            manager.Update(splitsReminders);
        }
    }
}
