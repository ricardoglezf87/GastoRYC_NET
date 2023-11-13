using GARCA.Data.Managers;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class SplitsRemindersService
    {
        private readonly SplitsRemindersManager splitsRemindersManager;

        public SplitsRemindersService()
        {
            splitsRemindersManager = new SplitsRemindersManager();
        }

        public HashSet<SplitsReminders>? GetbyTransactionidNull()
        {
            return splitsRemindersManager.GetbyTransactionidNull()?.ToHashSet();
        }

        public HashSet<SplitsReminders>? GetbyTransactionid(int transactionid)
        {
            return splitsRemindersManager.GetbyTransactionid(transactionid)?.ToHashSet();
        }

        public void Update(SplitsReminders splitsReminders)
        {
            splitsRemindersManager.Update(splitsReminders);
        }

        public void Delete(SplitsReminders splitsReminders)
        {
            splitsRemindersManager.Delete(splitsReminders);
        }
    }
}
