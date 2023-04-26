using BBDDLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GastosRYC.BBDDLib.Services
{
    public class ExpirationsRemindersService
    {
        private readonly SimpleInjector.Container servicesContainer;

        public ExpirationsRemindersService(SimpleInjector.Container servicesContainer)
        {
            this.servicesContainer = servicesContainer;
        }

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
            return (List<ExpirationsReminders>?)getAllWithGeneration()?.Where(x => x.done != true).ToList();
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
                    if (!existsExpiration(transactionsReminders, date))
                    {
                        ExpirationsReminders expirationsReminders = new ExpirationsReminders();
                        expirationsReminders.transactionsRemindersid = transactionsReminders.id;
                        expirationsReminders.transactionsReminders = transactionsReminders;
                        expirationsReminders.date = date;
                        update(expirationsReminders);
                    }

                    date = servicesContainer.GetInstance<PeriodsRemindersService>().getNextDate(date, servicesContainer.GetInstance<PeriodsRemindersService>().toEnum(transactionsReminders.periodsReminders));

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
                    servicesContainer.GetInstance<TransactionsService>().saveChanges(transactions);

                    if (expirationsReminders.transactionsReminders.splits != null)
                    {
                        foreach (SplitsReminders splitsReminders in expirationsReminders.transactionsReminders.splits)
                        {
                            Splits splits = new Splits();
                            splits.transactionid = transactions.id;
                            splits.categoryid = splitsReminders.categoryid;
                            splits.memo = splitsReminders.memo;
                            splits.amountIn = splitsReminders.amountIn;
                            splits.amountOut = splitsReminders.amountOut;
                            splits.tagid = splitsReminders.tagid;
                            servicesContainer.GetInstance<SplitsService>().saveChanges(transactions, splits);
                            servicesContainer.GetInstance<TransactionsService>().updateTranferSplits(transactions, splits);
                        }
                    }

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
