using BOLib.Models;
using BOLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using BOLib.Helpers;
using DAOLib.Managers;

namespace BOLib.Services
{
    public class ExpirationsRemindersService
    {
        private readonly ExpirationsRemindersManager expirationsRemindersManager;      
        private readonly PeriodsRemindersService periodsRemindersService;
        private readonly TransactionsService transactionsService;
        private readonly SplitsService splitsService;

        public ExpirationsRemindersService()
        {
            expirationsRemindersManager = new();
            periodsRemindersService = new();
            transactionsService = new();
            splitsService = new();
        }

        public List<ExpirationsReminders>? getAll()
        {
            return expirationsRemindersManager.getAll()?.toListBO();
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
            return getAllWithGeneration()?.Where(x => x.done != true).ToList();
        }

        public List<ExpirationsReminders>? getAllPendingWithoutFutureWithGeneration()
        {
            return getAllWithGeneration()?.Where(x => x.done != true && x.groupDate != "Futuro").ToList();
        }


        public void GenerationAllExpirations()
        {
            foreach (TransactionsReminders transactionsReminders in MapperConfig.InitializeAutomapper().Map<List<TransactionsReminders>>(RYCContextService.getInstance().BBDD.transactionsReminders))
            {
                generationExpirations(transactionsReminders);
            }
        }

        public void generateAutoregister()
        {
            foreach (ExpirationsReminders exp in getAllPendingWithGeneration()?
                .Where(x => x.date <= DateTime.Now &&
                    (x.transactionsReminders.autoRegister.HasValue && x.transactionsReminders.autoRegister.Value)))
            {
                registerTransactionfromReminder(exp.id);
                exp.done = true;
                update(exp);
            }
        }

        public void generationExpirations(TransactionsReminders? transactionsReminders)
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

                    date = periodsRemindersService.getNextDate(date, periodsRemindersService.toEnum(transactionsReminders.periodsReminders));

                }
            }
        }

        public Transactions? registerTransactionfromReminder(int? id)
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
                            splitsService.saveChanges(transactions, splits);
                            transactionsService.updateTranferSplits(transactions, splits);
                        }
                    }

                    return transactions;

                }
            }

            return null;
        }

        public List<Transactions> registerTransactionfromReminderSimulation(int id)
        {
            List<Transactions>? lTransactions = new();
            ExpirationsReminders? expirationsReminders = getByID(id);
            if (expirationsReminders != null && expirationsReminders.transactionsReminders != null)
            {
                Transactions transactions = new Transactions();
                transactions.date = expirationsReminders.date.removeTime();
                transactions.accountid = expirationsReminders.transactionsReminders.accountid;
                transactions.account = expirationsReminders.transactionsReminders.account;
                transactions.personid = expirationsReminders.transactionsReminders.personid;
                transactions.person = expirationsReminders.transactionsReminders.person;
                transactions.categoryid = expirationsReminders.transactionsReminders.categoryid;
                transactions.category = expirationsReminders.transactionsReminders.category;
                transactions.memo = expirationsReminders.transactionsReminders.memo;
                transactions.amountIn = expirationsReminders.transactionsReminders.amountIn ?? 0;
                transactions.amountOut = expirationsReminders.transactionsReminders.amountOut ?? 0;
                transactions.tag = expirationsReminders.transactionsReminders.tag;
                transactions.tagid = expirationsReminders.transactionsReminders.tagid;

                if (transactions.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
                {
                    lTransactions.Add(updateTranferSimulation(transactions));
                }

                if (expirationsReminders.transactionsReminders.splits != null)
                {

                    foreach (SplitsReminders splitsReminders in expirationsReminders.transactionsReminders.splits)
                    {
                        Splits splits = new Splits();
                        splits.transactionid = transactions.id;
                        splits.categoryid = splitsReminders.categoryid;
                        splits.category = splitsReminders.category;
                        splits.memo = splitsReminders.memo;
                        splits.amountIn = splitsReminders.amountIn ?? 0;
                        splits.amountOut = splitsReminders.amountOut ?? 0;
                        splits.tagid = splitsReminders.tagid;
                        if (splits.category.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Transfers)
                        {
                            lTransactions.Add(updateTranferSplitsSimulation(transactions, splits));
                        }
                    }
                }
                lTransactions.Add(transactions);
            }
            return lTransactions;

        }

        public Transactions updateTranferSplitsSimulation(Transactions? transactions, Splits splits)
        {
            //TODO:Revisar
            //Transactions? tContraria = new()
            //{
            //    date = transactions.date,
            //    accountid = splits.category.accounts.id,
            //    account = splits.category.accounts,
            //    personid = transactions.personid,
            //    person = transactions.person,
            //    categoryid = transactions.account.categoryid,
            //    category = servicesContainer.GetInstance<CategoriesService>().getByID(transactions.account.categoryid),
            //    memo = splits.memo,
            //    tagid = transactions.tagid,
            //    amountIn = splits.amountOut,
            //    amountOut = splits.amountIn
            //};

            //return tContraria;
            return null;
        }


        public Transactions updateTranferSimulation(Transactions transactions)
        {
            //TODO: revisar
            //Transactions? tContraria = new()
            //{
            //    date = transactions.date.removeTime(),
            //    accountid = transactions.category.accounts.id,
            //    account = transactions.category.accounts,
            //    personid = transactions.personid,
            //    person = transactions.person,
            //    categoryid = transactions.account.categoryid,
            //    category = servicesContainer.GetInstance<CategoriesService>().getByID(transactions.account.categoryid),
            //    memo = transactions.memo,
            //    tagid = transactions.tagid,
            //    tag = transactions.tag,
            //    amountIn = transactions.amountOut,
            //    amountOut = transactions.amountIn
            //};
            //return tContraria;
            return null;
        }

        public ExpirationsReminders? getByID(int? id)
        {
            return (ExpirationsReminders) expirationsRemindersManager.getByID(id);            
        }

        public List<ExpirationsReminders>? getByTransactionReminderid(int? id)
        {
            return expirationsRemindersManager.getByTransactionReminderid(id)?.toListBO();            
        }

        public void update(ExpirationsReminders expirationsReminders)
        {
            expirationsRemindersManager.update(expirationsReminders?.toDAO());
        }

        public void delete(ExpirationsReminders expirationsReminders)
        {
            expirationsRemindersManager.delete(expirationsReminders?.toDAO());
        }

        public void deleteByTransactionReminderid(int id)
        {
            foreach (ExpirationsReminders expirationsReminder in getByTransactionReminderid(id))
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
