using GARCA.BO.Models;
using GARCA.DAO.Managers;
using GARCA.Utils.IOC;
using GARCA.Utlis.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GARCA.BO.Services
{
    public class ExpirationsRemindersService
    {
        private readonly ExpirationsRemindersManager expirationsRemindersManager;

        public ExpirationsRemindersService()
        {
            expirationsRemindersManager = new ExpirationsRemindersManager();
        }

        private HashSet<ExpirationsReminders?>? GetAll()
        {
            return expirationsRemindersManager.GetAll()?.ToHashSetBo();
        }

        private HashSet<ExpirationsReminders?>? GetAllWithGeneration()
        {
            GenerationAllExpirations();
            return GetAll();
        }

        private bool ExistsExpiration(TransactionsReminders? transactionsReminder, DateTime? date)
        {
            return expirationsRemindersManager.ExistsExpiration(transactionsReminder.ToDao(), date);
        }

        private HashSet<ExpirationsReminders?>? GetAllPendingWithGeneration()
        {
            return GetAllWithGeneration()?.Where(x => x.Done is null or not true).ToHashSet();
        }

        public HashSet<ExpirationsReminders?>? GetAllPendingWithoutFutureWithGeneration()
        {
            return GetAllWithGeneration()?
                .Where(x => (x.Done == null || x.Done != true) && x.GroupDate != "Futuro").ToHashSet();
        }

        private void GenerationAllExpirations()
        {
            foreach (var transactionsReminders in DependencyConfig.TransactionsRemindersService.GetAll())
            {
                GenerationExpirations(transactionsReminders);
            }
        }

        public void GenerateAutoregister()
        {
            foreach (var exp in GetAllPendingWithGeneration()?
                .Where(x => x.Date <= DateTime.Now && //x.transactionsReminders != null &&
                    x.TransactionsReminders.AutoRegister.HasValue && x.TransactionsReminders.AutoRegister.Value))
            {
                RegisterTransactionfromReminder(exp.Id);
                exp.Done = true;
                Update(exp);
            }
        }

        private void GenerationExpirations(TransactionsReminders? transactionsReminders)
        {
            if (transactionsReminders != null)
            {
                var date = transactionsReminders.Date;

                while (date <= DateTime.Now.AddYears(1))
                {
                    if (!ExistsExpiration(transactionsReminders, date))
                    {
                        ExpirationsReminders expirationsReminders = new();
                        expirationsReminders.TransactionsRemindersid = transactionsReminders.Id;
                        expirationsReminders.TransactionsReminders = transactionsReminders;
                        expirationsReminders.Date = date;
                        Update(expirationsReminders);
                    }

                    date = DependencyConfig.PeriodsReminderService.GetNextDate(date, DependencyConfig.PeriodsReminderService.ToEnum(transactionsReminders.PeriodsReminders));

                }
            }
        }

        public Transactions? RegisterTransactionfromReminder(int? id)
        {
            if (id != null)
            {
                var expirationsReminders = GetById(id);
                if (expirationsReminders != null && expirationsReminders.TransactionsReminders != null)
                {
                    Transactions? transactions = new();
                    transactions.Date = expirationsReminders.Date;
                    transactions.Accountid = expirationsReminders.TransactionsReminders.Accountid;
                    transactions.Personid = expirationsReminders.TransactionsReminders.Personid;
                    transactions.Categoryid = expirationsReminders.TransactionsReminders.Categoryid;
                    transactions.Category = expirationsReminders.TransactionsReminders.Category;
                    transactions.Memo = expirationsReminders.TransactionsReminders.Memo;
                    transactions.AmountIn = expirationsReminders.TransactionsReminders.AmountIn;
                    transactions.AmountOut = expirationsReminders.TransactionsReminders.AmountOut;
                    transactions.Tagid = expirationsReminders.TransactionsReminders.Tagid;
                    transactions.TransactionStatusid = (int)TransactionsStatusService.ETransactionsTypes.Pending;
                    DependencyConfig.TransactionsService.SaveChanges(ref transactions);

                    foreach (var splitsReminders in
                        DependencyConfig.SplitsRemindersService.GetbyTransactionid(expirationsReminders.TransactionsReminders.Id))
                    {
                        Splits splits = new();
                        splits.Transactionid = transactions.Id;
                        splits.Categoryid = splitsReminders.Categoryid;
                        splits.Memo = splitsReminders.Memo;
                        splits.AmountIn = splitsReminders.AmountIn;
                        splits.AmountOut = splitsReminders.AmountOut;
                        splits.Tagid = splitsReminders.Tagid;
                        DependencyConfig.SplitsService.SaveChanges(splits);
                        DependencyConfig.TransactionsService.UpdateTranferSplits(transactions, splits);
                    }

                    return transactions;

                }
            }

            return null;
        }

        public HashSet<Transactions> RegisterTransactionfromReminderSimulation(ExpirationsReminders exp)
        {
            HashSet<Transactions> lTransactions = new();
            var expirationsReminders = exp;
            if (expirationsReminders.TransactionsReminders != null)
            {
                Transactions transactions = new()
                {
                    Date = expirationsReminders.Date.RemoveTime(),
                    Accountid = expirationsReminders.TransactionsReminders.Accountid,
                    Account = expirationsReminders.TransactionsReminders.Account,
                    Personid = expirationsReminders.TransactionsReminders.Personid,
                    Person = expirationsReminders.TransactionsReminders.Person,
                    Categoryid = expirationsReminders.TransactionsReminders.Categoryid,
                    Category = expirationsReminders.TransactionsReminders.Category,
                    Memo = expirationsReminders.TransactionsReminders.Memo,
                    AmountIn = expirationsReminders.TransactionsReminders.AmountIn ?? 0,
                    AmountOut = expirationsReminders.TransactionsReminders.AmountOut ?? 0,
                    Tag = expirationsReminders.TransactionsReminders.Tag,
                    Tagid = expirationsReminders.TransactionsReminders.Tagid
                };

                if (transactions.Category.CategoriesTypesid == (int)CategoriesTypesService.ECategoriesTypes.Transfers)
                {
                    lTransactions.Add(UpdateTranferSimulation(transactions));
                }

                if (expirationsReminders.TransactionsReminders.Splits != null)
                {

                    foreach (var splitsReminders in expirationsReminders.TransactionsReminders.Splits)
                    {
                        Splits splits = new()
                        {
                            Transactionid = transactions.Id,
                            Categoryid = splitsReminders.Categoryid,
                            Category = splitsReminders.Category,
                            Memo = splitsReminders.Memo,
                            AmountIn = splitsReminders.AmountIn ?? 0,
                            AmountOut = splitsReminders.AmountOut ?? 0,
                            Tagid = splitsReminders.Tagid
                        };
                        if (splits.Category.CategoriesTypesid == (int)CategoriesTypesService.ECategoriesTypes.Transfers)
                        {
                            lTransactions.Add(UpdateTranferSplitsSimulation(transactions, splits));
                        }
                    }
                }
                lTransactions.Add(transactions);
            }
            return lTransactions;
        }

        private Transactions UpdateTranferSplitsSimulation(Transactions? transactions, Splits splits)
        {
            Transactions tContraria = new()
            {
                Date = transactions.Date,
                Accountid = DependencyConfig.AccountsService.GetByCategoryId(splits.Categoryid)?.Id,
                Account = DependencyConfig.AccountsService.GetByCategoryId(splits.Categoryid),
                Personid = transactions.Personid,
                Person = transactions.Person,
                Categoryid = transactions.Account.Categoryid,
                Category = DependencyConfig.CategoriesService.GetById(transactions.Account.Categoryid),
                Memo = splits.Memo,
                Tagid = transactions.Tagid,
                AmountIn = splits.AmountOut,
                AmountOut = splits.AmountIn
            };

            return tContraria;
        }


        private Transactions UpdateTranferSimulation(Transactions transactions)
        {
            Transactions tContraria = new()
            {
                Date = transactions.Date.RemoveTime(),
                Accountid = DependencyConfig.AccountsService.GetByCategoryId(transactions.Categoryid)?.Id,
                Account = DependencyConfig.AccountsService.GetByCategoryId(transactions.Categoryid),
                Personid = transactions.Personid,
                Person = transactions.Person,
                Categoryid = transactions.Account?.Categoryid,
                Category = DependencyConfig.CategoriesService.GetById(transactions.Account?.Categoryid),
                Memo = transactions.Memo,
                Tagid = transactions.Tagid,
                Tag = transactions.Tag,
                AmountIn = transactions.AmountOut,
                AmountOut = transactions.AmountIn
            };
            return tContraria;
        }

        public ExpirationsReminders? GetById(int? id)
        {
            return (ExpirationsReminders?)expirationsRemindersManager.GetById(id);
        }

        private HashSet<ExpirationsReminders?>? GetByTransactionReminderid(int? id)
        {
            return expirationsRemindersManager.GetByTransactionReminderid(id)?.ToHashSetBo();
        }

        public void Update(ExpirationsReminders expirationsReminders)
        {
            expirationsRemindersManager.Update(expirationsReminders.ToDao());
        }

        private void Delete(ExpirationsReminders? expirationsReminders)
        {
            expirationsRemindersManager.Delete(expirationsReminders?.ToDao());
        }

        public void DeleteByTransactionReminderid(int id)
        {
            foreach (var expirationsReminder in GetByTransactionReminderid(id))
            {
                Delete(expirationsReminder);
            }
        }

        public DateTime? GetNextReminder(int id)
        {
            return GetByTransactionReminderid(id)?.Where(x => !x.Done.HasValue || !x.Done.Value).Min(y => y.Date);
        }
    }
}
