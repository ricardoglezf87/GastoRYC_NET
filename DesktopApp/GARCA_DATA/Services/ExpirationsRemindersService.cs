using GARCA.Data.Managers;
using GARCA.Models;
using GARCA.Utils.Extensions;
using static GARCA.Data.IOC.DependencyConfig;


namespace GARCA.Data.Services
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
            return expirationsRemindersManager.GetAll()?.ToHashSet();
        }

        private HashSet<ExpirationsReminders?>? GetAllWithGeneration()
        {
            GenerationAllExpirations();
            return GetAll();
        }

        private bool ExistsExpiration(TransactionsReminders? transactionsReminder, DateTime? date)
        {
            return expirationsRemindersManager.ExistsExpiration(transactionsReminder, date);
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
            foreach (var transactionsReminders in iTransactionsRemindersService.GetAll())
            {
                GenerationExpirations(transactionsReminders);
            }
        }

        public async Task GenerateAutoregister()
        {
            foreach (var exp in GetAllPendingWithGeneration()?
                .Where(x => x.Date <= DateTime.Now && //x.transactionsReminders != null &&
                    x.TransactionsReminders.AutoRegister.HasValue && x.TransactionsReminders.AutoRegister.Value))
            {
                await RegisterTransactionfromReminder(exp.Id);
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

                    date = iPeriodsReminderService.GetNextDate(date, iPeriodsReminderService.ToEnum(transactionsReminders.PeriodsReminders));

                }
            }
        }

        public async Task<Transactions?> RegisterTransactionfromReminder(int? id)
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
                    transactions = await iTransactionsService.SaveChanges(transactions);

                    foreach (var splitsReminders in
                        iSplitsRemindersService.GetbyTransactionid(expirationsReminders.TransactionsReminders.Id))
                    {
                        Splits splits = new();
                        splits.Transactionid = transactions.Id;
                        splits.Categoryid = splitsReminders.Categoryid;
                        splits.Memo = splitsReminders.Memo;
                        splits.AmountIn = splitsReminders.AmountIn;
                        splits.AmountOut = splitsReminders.AmountOut;
                        splits.Tagid = splitsReminders.Tagid;

                        iTransactionsService.UpdateTranferSplits(transactions, ref splits);
                        iSplitsService.SaveChanges(splits);
                    }

                    await iTransactionsService.RefreshBalanceAllTransactions();

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
                Accountid = iAccountsService.GetByCategoryId(splits.Categoryid)?.Id,
                Account = iAccountsService.GetByCategoryId(splits.Categoryid),
                Personid = transactions.Personid,
                Person = transactions.Person,
                Categoryid = transactions.Account.Categoryid,
                Category = iCategoriesService.GetById(transactions.Account.Categoryid),
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
                Accountid = iAccountsService.GetByCategoryId(transactions.Categoryid)?.Id,
                Account = iAccountsService.GetByCategoryId(transactions.Categoryid),
                Personid = transactions.Personid,
                Person = transactions.Person,
                Categoryid = transactions.Account?.Categoryid,
                Category = iCategoriesService.GetById(transactions.Account?.Categoryid),
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
            return expirationsRemindersManager.GetById(id);
        }

        private HashSet<ExpirationsReminders?>? GetByTransactionReminderid(int? id)
        {
            return expirationsRemindersManager.GetByTransactionReminderid(id)?.ToHashSet();
        }

        public void Update(ExpirationsReminders expirationsReminders)
        {
            expirationsRemindersManager.Update(expirationsReminders);
        }

        private void Delete(ExpirationsReminders? expirationsReminders)
        {
            expirationsRemindersManager.Delete(expirationsReminders);
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
