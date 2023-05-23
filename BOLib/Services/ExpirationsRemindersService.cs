using BOLib.Extensions;
using BOLib.Models;
using DAOLib.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOLib.Services
{
    public class ExpirationsRemindersService
    {
        private readonly ExpirationsRemindersManager expirationsRemindersManager;
        private static ExpirationsRemindersService? _instance;
        private static readonly object _lock = new();

        public static ExpirationsRemindersService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new ExpirationsRemindersService();
                    }
                }
                return _instance;
            }
        }

        private ExpirationsRemindersService()
        {
            expirationsRemindersManager = new();
        }

        public List<ExpirationsReminders?>? getAll()
        {
            return expirationsRemindersManager.getAll()?.toListBO();
        }

        public List<ExpirationsReminders?>? getAllWithGeneration()
        {
            GenerationAllExpirations();
            return getAll();
        }

        public bool existsExpiration(TransactionsReminders? transactionsReminder, DateTime? date)
        {
            return expirationsRemindersManager.existsExpiration(transactionsReminder.toDAO(), date);
        }

        public List<ExpirationsReminders?>? getAllPendingWithGeneration()
        {
            return getAllWithGeneration()?.Where(x => x.done != true).ToList();
        }

        public List<ExpirationsReminders?>? getAllPendingWithoutFutureWithGeneration()
        {
            return getAllWithGeneration()?.Where(x => x.done != true && x.groupDate != "Futuro").ToList();
        }

        public async Task<List<ExpirationsReminders?>?> getAllPendingWithoutFutureWithGenerationAsync()
        {
            return await Task.Run(() => getAllPendingWithoutFutureWithGeneration());
        }

        public void GenerationAllExpirations()
        {
            foreach (TransactionsReminders? transactionsReminders in TransactionsRemindersService.Instance.getAll())
            {
                generationExpirations(transactionsReminders);
            }
        }

        public void generateAutoregister()
        {
            foreach (ExpirationsReminders? exp in getAllPendingWithGeneration()?
                .Where(x => x.date <= DateTime.Now && //x.transactionsReminders != null &&
                    x.transactionsReminders.autoRegister.HasValue && x.transactionsReminders.autoRegister.Value))
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
                        ExpirationsReminders expirationsReminders = new();
                        expirationsReminders.transactionsRemindersid = transactionsReminders.id;
                        expirationsReminders.transactionsReminders = transactionsReminders;
                        expirationsReminders.date = date;
                        update(expirationsReminders);
                    }

                    date = PeriodsRemindersService.Instance.getNextDate(date, PeriodsRemindersService.Instance.toEnum(transactionsReminders.periodsReminders));

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
                    Transactions? transactions = new();
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
                    TransactionsService.Instance.saveChanges(ref transactions);
                   
                    foreach (SplitsReminders? splitsReminders in 
                        SplitsRemindersService.Instance.getbyTransactionid(expirationsReminders.transactionsReminders.id))
                    {
                        Splits splits = new();
                        splits.transactionid = transactions.id;
                        splits.categoryid = splitsReminders.categoryid;
                        splits.memo = splitsReminders.memo;
                        splits.amountIn = splitsReminders.amountIn;
                        splits.amountOut = splitsReminders.amountOut;
                        splits.tagid = splitsReminders.tagid;
                        SplitsService.Instance.saveChanges(transactions, splits);
                        TransactionsService.Instance.updateTranferSplits(transactions, splits);
                    }

                    return transactions;

                }
            }

            return null;
        }

        public List<Transactions> registerTransactionfromReminderSimulation(ExpirationsReminders exp)
        {
            List<Transactions>? lTransactions = new();
            ExpirationsReminders? expirationsReminders = exp;
            if (expirationsReminders != null && expirationsReminders.transactionsReminders != null)
            {
                Transactions transactions = new();
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

                    foreach (SplitsReminders? splitsReminders in expirationsReminders.transactionsReminders.splits)
                    {
                        Splits splits = new();
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

        public Task<List<Transactions>> registerTransactionfromReminderSimulationAsync(ExpirationsReminders exp)
        {
            return Task.Run(() => registerTransactionfromReminderSimulation(exp));
        }

        public Transactions updateTranferSplitsSimulation(Transactions? transactions, Splits splits)
        {
            Transactions? tContraria = new()
            {
                date = transactions.date,
                accountid = AccountsService.Instance.getByCategoryId(splits.categoryid)?.id,
                account = AccountsService.Instance.getByCategoryId(splits.categoryid),
                personid = transactions.personid,
                person = transactions.person,
                categoryid = transactions.account.categoryid,
                category = CategoriesService.Instance.getByID(transactions.account.categoryid),
                memo = splits.memo,
                tagid = transactions.tagid,
                amountIn = splits.amountOut,
                amountOut = splits.amountIn
            };

            return tContraria;
        }


        public Transactions updateTranferSimulation(Transactions transactions)
        {
            Transactions? tContraria = new()
            {
                date = transactions.date.removeTime(),
                accountid = AccountsService.Instance.getByCategoryId(transactions.categoryid)?.id,
                account = AccountsService.Instance.getByCategoryId(transactions.categoryid),
                personid = transactions.personid,
                person = transactions.person,
                categoryid = transactions.account?.categoryid,
                category = CategoriesService.Instance.getByID(transactions.account?.categoryid),
                memo = transactions.memo,
                tagid = transactions.tagid,
                tag = transactions.tag,
                amountIn = transactions.amountOut,
                amountOut = transactions.amountIn
            };
            return tContraria;
        }

        public ExpirationsReminders? getByID(int? id)
        {
            return (ExpirationsReminders?)expirationsRemindersManager.getByID(id);
        }

        public List<ExpirationsReminders?>? getByTransactionReminderid(int? id)
        {
            return expirationsRemindersManager.getByTransactionReminderid(id)?.toListBO();
        }

        public void update(ExpirationsReminders expirationsReminders)
        {
            expirationsRemindersManager.update(expirationsReminders?.toDAO());
        }

        public void delete(ExpirationsReminders? expirationsReminders)
        {
            expirationsRemindersManager.delete(expirationsReminders?.toDAO());
        }

        public void deleteByTransactionReminderid(int id)
        {
            foreach (ExpirationsReminders? expirationsReminder in getByTransactionReminderid(id))
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
