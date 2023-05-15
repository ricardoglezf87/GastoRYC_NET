using BOLib.Extensions;

using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;

namespace BOLib.Services
{
    public class SplitsRemindersService
    {
        private readonly SplitsRemindersManager splitsRemindersManager;

        public SplitsRemindersService()
        {
            splitsRemindersManager = InstanceBase<SplitsRemindersManager>.Instance;
        }

        public List<SplitsReminders?>? getAll()
        {
            return splitsRemindersManager.getAll()?.toListBO();
        }

        public List<SplitsReminders?>? getbyTransactionidNull()
        {
            return splitsRemindersManager.getbyTransactionidNull()?.toListBO();
        }

        public List<SplitsReminders?>? getbyTransactionid(int transactionid)
        {
            return splitsRemindersManager.getbyTransactionid(transactionid)?.toListBO();
        }

        public SplitsReminders? getByID(int? id)
        {
            return (SplitsReminders?)splitsRemindersManager.getByID(id);
        }

        public void update(SplitsReminders splitsReminders)
        {
            splitsRemindersManager.update(splitsReminders.toDAO());
        }

        public void delete(SplitsReminders splitsReminders)
        {
            splitsRemindersManager.delete(splitsReminders.toDAO());
        }

        public int getNextID()
        {
            return splitsRemindersManager.getNextID();
        }
    }
}
