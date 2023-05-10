using DAOLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Services
{
    public class ExpirationsRemindersServiceDAO
    {
        private readonly SimpleInjector.Container servicesContainer;

        public ExpirationsRemindersServiceDAO(SimpleInjector.Container servicesContainer)
        {
            this.servicesContainer = servicesContainer;
        }

        public List<ExpirationsRemindersDAO>? getAll()
        {
            return RYCContextServiceDAO.getInstance().BBDD.expirationsReminders?.ToList();
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

        public ExpirationsRemindersDAO? getByID(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.expirationsReminders?.FirstOrDefault(x => id.Equals(x.id));
        }

        public List<ExpirationsRemindersDAO>? getByTransactionReminderid(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.expirationsReminders?.Where(x => id.Equals(x.transactionsRemindersid)).ToList();
        }

        public void update(ExpirationsRemindersDAO ExpirationsRemindersDAO)
        {
            RYCContextServiceDAO.getInstance().BBDD.Update(ExpirationsRemindersDAO);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

        public void delete(ExpirationsRemindersDAO ExpirationsRemindersDAO)
        {
            RYCContextServiceDAO.getInstance().BBDD.Remove(ExpirationsRemindersDAO);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
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
