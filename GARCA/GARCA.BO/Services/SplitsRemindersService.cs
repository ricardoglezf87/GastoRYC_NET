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
            splitsRemindersManager = new();
        }

        public List<SplitsReminders?>? getbyTransactionidNull()
        {
            return splitsRemindersManager.getbyTransactionidNull()?.toListBO();
        }

        public List<SplitsReminders?>? getbyTransactionid(int transactionid)
        {
            return splitsRemindersManager.getbyTransactionid(transactionid)?.toListBO();
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
