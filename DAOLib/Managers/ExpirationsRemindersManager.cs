using DAOLib.Models;
using DAOLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Managers
{
    public class ExpirationsRemindersManager : IManager<ExpirationsRemindersDAO>
    {
        private readonly SimpleInjector.Container servicesContainer;

        public ExpirationsRemindersManager(SimpleInjector.Container servicesContainer)
        {
            this.servicesContainer = servicesContainer;
        }

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

        public void deleteByTransactionReminderid(int id)
        {
            foreach (ExpirationsRemindersDAO expirationsReminder in getByTransactionReminderid(id))
            {
                delete(expirationsReminder);
            }
        }

        public DateTime? getNextReminder(int id)
        {
            return getByTransactionReminderid(id)?.Where(x => !x.done.HasValue || !x.done.Value).Min(y => y.date);
        }
    }
}
