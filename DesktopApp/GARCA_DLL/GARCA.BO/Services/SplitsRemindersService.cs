using GARCA.BO.Models;
using GARCA.DAO.Managers;
using GARCA.Utlis.Extensions;

namespace GARCA.BO.Services
{
    public class SplitsRemindersService
    {
        private readonly SplitsRemindersManager splitsRemindersManager;

        public SplitsRemindersService()
        {
            splitsRemindersManager = new SplitsRemindersManager();
        }

        public HashSet<SplitsReminders?>? GetbyTransactionidNull()
        {
            return splitsRemindersManager.GetbyTransactionidNull()?.ToHashSetBo();
        }

        public HashSet<SplitsReminders?>? GetbyTransactionid(int transactionid)
        {
            return splitsRemindersManager.GetbyTransactionid(transactionid)?.ToHashSetBo();
        }

        public void Update(SplitsReminders splitsReminders)
        {
            splitsRemindersManager.Update(splitsReminders.ToDao());
        }

        public void Delete(SplitsReminders splitsReminders)
        {
            splitsRemindersManager.Delete(splitsReminders.ToDao());
        }
    }
}
