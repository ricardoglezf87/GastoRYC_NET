using GARCA.Utlis.Extensions;
using GARCA.BO.Models;
using GARCA.DAO.Managers;
using GARCA.Utils.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GARCA.BO.Services
{
    public class ExpirationsRemindersService
    {
        private readonly ExpirationsRemindersManager expirationsRemindersManager;

        public ExpirationsRemindersService()
        {
            expirationsRemindersManager = new ExpirationsRemindersManager();
        }

        private HashSet<ExpirationsReminders?>? getAll()
        {
            return expirationsRemindersManager.getAll()?.toHashSetBO();
        }

        private HashSet<ExpirationsReminders?>? getAllWithGeneration()
        {
            GenerationAllExpirations();
            return getAll();
        }

        private bool existsExpiration(TransactionsReminders? transactionsReminder, DateTime? date)
        {
            return expirationsRemindersManager.existsExpiration(transactionsReminder.toDAO(), date);
        }

        private List<ExpirationsReminders?>? getAllPendingWithGeneration()
        {
            return getAllWithGeneration()?.Where(x => x.done is null or not true).ToList();
        }

        public List<ExpirationsReminders?>? getAllPendingWithoutFutureWithGeneration()
        {
            return getAllWithGeneration()?.Where(x => (x.done == null || x.done != true) && x.groupDate != "Futuro").ToList();
        }

        private void GenerationAllExpirations()
        {
            foreach (var transactionsReminders in DependencyConfig.iTransactionsRemindersService.getAll())
            {
                generationExpirations(transactionsReminders);
            }
        }

        public void generateAutoregister()
        {
            foreach (var exp in getAllPendingWithGeneration()?
                .Where(x => x.date <= DateTime.Now && //x.transactionsReminders != null &&
                    x.transactionsReminders.autoRegister.HasValue && x.transactionsReminders.autoRegister.Value))
            {
                registerTransactionfromReminder(exp.id);
                exp.done = true;
                update(exp);
            }
        }

        private void generationExpirations(TransactionsReminders? transactionsReminders)
        {
            if (transactionsReminders != null)
            {
                var date = transactionsReminders.date;

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

                    date = DependencyConfig.iPeriodsReminderService.getNextDate(date, DependencyConfig.iPeriodsReminderService.toEnum(transactionsReminders.periodsReminders));

                }
            }
        }

        public Transactions? registerTransactionfromReminder(int? id)
        {
            if (id != null)
            {
                var expirationsReminders = getByID(id);
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
                    DependencyConfig.iTransactionsService.saveChanges(ref transactions);

                    foreach (var splitsReminders in
                        DependencyConfig.iSplitsRemindersService.getbyTransactionid(expirationsReminders.transactionsReminders.id))
                    {
                        Splits splits = new();
                        splits.transactionid = transactions.id;
                        splits.categoryid = splitsReminders.categoryid;
                        splits.memo = splitsReminders.memo;
                        splits.amountIn = splitsReminders.amountIn;
                        splits.amountOut = splitsReminders.amountOut;
                        splits.tagid = splitsReminders.tagid;
                        DependencyConfig.iSplitsService.saveChanges(splits);
                        DependencyConfig.iTransactionsService.updateTranferSplits(transactions, splits);
                    }

                    return transactions;

                }
            }

            return null;
        }

        public List<Transactions> registerTransactionfromReminderSimulation(ExpirationsReminders exp)
        {
            List<Transactions>? lTransactions = new();
            var expirationsReminders = exp;
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

                    foreach (var splitsReminders in expirationsReminders.transactionsReminders.splits)
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

        private Transactions updateTranferSplitsSimulation(Transactions? transactions, Splits splits)
        {
            Transactions? tContraria = new()
            {
                date = transactions.date,
                accountid = DependencyConfig.iAccountsService.getByCategoryId(splits.categoryid)?.id,
                account = DependencyConfig.iAccountsService.getByCategoryId(splits.categoryid),
                personid = transactions.personid,
                person = transactions.person,
                categoryid = transactions.account.categoryid,
                category = DependencyConfig.iCategoriesService.getByID(transactions.account.categoryid),
                memo = splits.memo,
                tagid = transactions.tagid,
                amountIn = splits.amountOut,
                amountOut = splits.amountIn
            };

            return tContraria;
        }


        private Transactions updateTranferSimulation(Transactions transactions)
        {
            Transactions? tContraria = new()
            {
                date = transactions.date.removeTime(),
                accountid = DependencyConfig.iAccountsService.getByCategoryId(transactions.categoryid)?.id,
                account = DependencyConfig.iAccountsService.getByCategoryId(transactions.categoryid),
                personid = transactions.personid,
                person = transactions.person,
                categoryid = transactions.account?.categoryid,
                category = DependencyConfig.iCategoriesService.getByID(transactions.account?.categoryid),
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

        private List<ExpirationsReminders?>? getByTransactionReminderid(int? id)
        {
            return expirationsRemindersManager.getByTransactionReminderid(id)?.toListBO();
        }

        public void update(ExpirationsReminders expirationsReminders)
        {
            expirationsRemindersManager.update(expirationsReminders?.toDAO());
        }

        private void delete(ExpirationsReminders? expirationsReminders)
        {
            expirationsRemindersManager.delete(expirationsReminders?.toDAO());
        }

        public void deleteByTransactionReminderid(int id)
        {
            foreach (var expirationsReminder in getByTransactionReminderid(id))
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
