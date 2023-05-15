using DAOLib.Models;
using DAOLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Managers
{
    public class ExpirationsRemindersManager : ManagerBase<ExpirationsRemindersDAO>
    {
        public bool existsExpiration(TransactionsRemindersDAO? transactionsReminder, DateTime? date)
        {
            if (transactionsReminder == null)
            {
                return false;
            }
            return RYCContextServiceDAO.getInstance().BBDD.expirationsReminders?
                    .Any(x => x.transactionsRemindersid == transactionsReminder.id && x.date == date) ?? false;
        }

        public List<ExpirationsRemindersDAO>? getByTransactionReminderid(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.expirationsReminders?.Where(x => id.Equals(x.transactionsRemindersid)).ToList();
        }
    }
}
