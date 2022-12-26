using BBDDLib.Models;
using BBDDLib.Models.Charts;
using GastosRYC.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class ExpirationsRemindersService
    {
        private TransactionsRemindersService transactionsRemindersService = new TransactionsRemindersService();
        private PeriodsRemindersService periodsRemindersService = new PeriodsRemindersService();

        public List<ExpirationsReminders>? getAll()
        {
            return RYCContextService.getInstance().BBDD.expirationsReminders?.ToList();
        }

        public List<ExpirationsReminders>? getAllWithGeneration()
        {
            GenerationAllExpirations();
            return RYCContextService.getInstance().BBDD.expirationsReminders?.ToList();
        }

        public bool existsExpiration(TransactionsReminders? transactionsReminder, DateTime? date)
        {
            if (transactionsReminder == null)
            {
                return false;
            }
            return RYCContextService.getInstance().BBDD.expirationsReminders?
                    .Any(x => x.transactaionsRemindersid == transactionsReminder.id && x.date == date) ?? false;
        }


        public void GenerationAllExpirations()
        {
            foreach (TransactionsReminders transactionsReminders in RYCContextService.getInstance().BBDD.transactionsReminders)
            {
                GenerationExpirations(transactionsReminders);                
            }
        }

        public void GenerationExpirations(TransactionsReminders? transactionsReminders)
        {
            if (transactionsReminders != null)
            {
                DateTime? date = transactionsReminders.date;

                while (date <= DateTime.Now.AddYears(1))
                {
                    if(!existsExpiration(transactionsReminders,date))
                    {
                        ExpirationsReminders expirationsReminders = new ExpirationsReminders();
                        expirationsReminders.transactaionsRemindersid = transactionsReminders.id;
                        expirationsReminders.transactaionsReminders = transactionsReminders;
                        expirationsReminders.date = date;
                        update(expirationsReminders);
                    }

                    date = periodsRemindersService.getNextDate(date, periodsRemindersService.toEnum(transactionsReminders.periodsReminders));

                }
            }
        }

        public ExpirationsReminders? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.expirationsReminders?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(ExpirationsReminders expirationsReminders)
        {
            RYCContextService.getInstance().BBDD.Update(expirationsReminders);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(ExpirationsReminders expirationsReminders)
        {
            RYCContextService.getInstance().BBDD.Remove(expirationsReminders);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }
    }
}
