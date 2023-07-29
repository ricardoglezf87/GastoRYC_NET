using GARCA.Utlis.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class SplitsRemindersService
    {
        private readonly SplitsRemindersManager splitsRemindersManager;

        public SplitsRemindersService()
        {
            splitsRemindersManager = new SplitsRemindersManager();
        }

        public HashSet<SplitsReminders?>? getbyTransactionidNull()
        {
            return splitsRemindersManager.getbyTransactionidNull()?.toHashSetBO();
        }

        public HashSet<SplitsReminders?>? getbyTransactionid(int transactionid)
        {
            return splitsRemindersManager.getbyTransactionid(transactionid)?.toHashSetBO();
        }

        public void update(SplitsReminders splitsReminders)
        {
            splitsRemindersManager.update(splitsReminders.toDAO());
        }

        public void delete(SplitsReminders splitsReminders)
        {
            splitsRemindersManager.delete(splitsReminders.toDAO());
        }
    }
}
