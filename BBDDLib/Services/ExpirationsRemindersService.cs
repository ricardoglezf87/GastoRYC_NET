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
        private TransactionsService transactionsService = new TransactionsService();
        //private TransactionsRemindersService transactionsRemindersService = new TransactionsRemindersService();
        private PeriodsRemindersService periodsRemindersService = new PeriodsRemindersService();

        public List<ExpirationsReminders>? getAll()
        {
            return RYCContextService.getInstance().BBDD.expirationsReminders?.ToList();
        }

        public List<ExpirationsReminders>? getAllWithGeneration()
        {
            GenerationAllExpirations();
            return getAll();
        }

        public bool existsExpiration(TransactionsReminders? transactionsReminder, DateTime? date)
        {
            if (transactionsReminder == null)
            {
                return false;
            }
            return RYCContextService.getInstance().BBDD.expirationsReminders?
                    .Any(x => x.transactionsRemindersid == transactionsReminder.id && x.date == date) ?? false;
        }

        public List<ExpirationsReminders>? getAllPendingWithGeneration()
        {        
            return (List<ExpirationsReminders>?) getAllWithGeneration()?.Where(x=> x.done != true).ToList();
        }

        public List<ExpirationsReminders>? getAllPendingWithoutFutureWithGeneration()
        {
            return (List<ExpirationsReminders>?)getAllWithGeneration()?.Where(x => x.done != true && x.groupDate != "Futuro").ToList();
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
                        expirationsReminders.transactionsRemindersid = transactionsReminders.id;
                        expirationsReminders.transactionsReminders = transactionsReminders;
                        expirationsReminders.date = date;
                        update(expirationsReminders);
                    }

                    date = periodsRemindersService.getNextDate(date, periodsRemindersService.toEnum(transactionsReminders.periodsReminders));

                }
            }
        }

        public void registerTransactionfromReminder(int? id)
        {
            if (id != null)
            {
                ExpirationsReminders? expirationsReminders = getByID(id);
                if (expirationsReminders != null && expirationsReminders.transactionsReminders != null)
                {
                    Transactions transactions = new Transactions();
                    transactions.date = expirationsReminders.date;
                    transactions.accountid = expirationsReminders.transactionsReminders.accountid;                    
                    transactions.personid = expirationsReminders.transactionsReminders.personid;
                    transactions.categoryid = expirationsReminders.transactionsReminders.categoryid;
                    transactions.category = expirationsReminders.transactionsReminders.category;
                    transactions.memo = expirationsReminders.transactionsReminders.memo;
                    transactions.amountIn = expirationsReminders.transactionsReminders.amountIn;
                    transactions.amountOut = expirationsReminders.transactionsReminders.amountOut;
                    transactions.tagid = expirationsReminders.transactionsReminders.tagid;
                    transactions.transactionStatusid = (int)TransactionsStatusService.eTransactionsTypes.Pending;

                    transactionsService.saveChanges(transactions);

                    //TODO: Falta implementar los splits
                }
            }
        }

        public ExpirationsReminders? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.expirationsReminders?.FirstOrDefault(x => id.Equals(x.id));
        }

        public List<ExpirationsReminders>? getByTransactionReminderid(int? id)
        {
            return RYCContextService.getInstance().BBDD.expirationsReminders?.Where(x => id.Equals(x.transactionsRemindersid)).ToList();
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

        public void deleteByTransactionReminderid(int id)
        {
            foreach (ExpirationsReminders expirationsReminder in getByTransactionReminderid(id))
            {
                delete(expirationsReminder);
            }
        }
    }
}
