using GARCA.Data.Managers;
using GARCA.Models;
using GARCA.Utils.Extensions;
using GARCA_DATA.Managers;
using static GARCA.Data.IOC.DependencyConfig;


namespace GARCA.Data.Services
{
    public class ExpirationsRemindersService : ServiceBase<ExpirationsRemindersManager, ExpirationsReminders, Int32>
    {
        private async Task<IEnumerable<ExpirationsReminders>?> GetAllWithGeneration()
        {
            GenerationAllExpirations();
            return await GetAll();
        }

        private async Task<bool> ExistsExpiration(TransactionsReminders transactionsReminder, DateTime date)
        {
            return await manager.ExistsExpiration(transactionsReminder, date);
        }

        private async Task<IEnumerable<ExpirationsReminders>?> GetAllPendingWithGeneration()
        {
            return (await GetAllWithGeneration())?.Where(x => x.Done is not true);
        }

        public async Task<IEnumerable<ExpirationsReminders>?> GetAllPendingWithoutFutureWithGeneration()
        {
            return (await GetAllWithGeneration())?
                .Where(x => (x.Done == null || x.Done != true) && x.GroupDate != "Futuro");
        }

        private async Task GenerationAllExpirations()
        {
            foreach (var transactionsReminders in await iTransactionsRemindersService.GetAll())
            {
                GenerationExpirations(transactionsReminders);
            }
        }

        public async Task GenerateAutoregister()
        {
            foreach (var exp in (await GetAllPendingWithGeneration() ?? Enumerable.Empty<ExpirationsReminders>())
                .Where(x => x.Date <= DateTime.Now && //x.transactionsReminders != null &&
                    x.TransactionsReminders.AutoRegister.HasValue && x.TransactionsReminders.AutoRegister.Value))
            {
                await RegisterTransactionfromReminder(exp.Id);
                exp.Done = true;
                Update(exp);
            }
        }

        private async Task GenerationExpirations(TransactionsReminders transactionsReminders)
        {
            if (transactionsReminders != null)
            {
                var date = transactionsReminders.Date;

                while (date <= DateTime.Now.AddYears(1))
                {
                    if (!await ExistsExpiration(transactionsReminders, date ?? DateTime.MinValue))
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
                var expirationsReminders = await GetById(id ?? -99);
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
                        await iSplitsRemindersService.GetbyTransactionid(expirationsReminders.TransactionsReminders.Id))
                    {
                        Splits splits = new();
                        splits.Transactionid = transactions.Id;
                        splits.Categoryid = splitsReminders.Categoryid;
                        splits.Memo = splitsReminders.Memo;
                        splits.AmountIn = splitsReminders.AmountIn;
                        splits.AmountOut = splitsReminders.AmountOut;
                        splits.Tagid = splitsReminders.Tagid;
                        
                        splits = await iTransactionsService.UpdateTranferSplits(transactions, splits);
                        await iSplitsService.SaveChanges(splits);
                    }

                    await iTransactionsService.RefreshBalanceAllTransactions();

                    return transactions;
                }
            }

            return null;
        }

        public async Task<IEnumerable<Transactions>> RegisterTransactionfromReminderSimulation(ExpirationsReminders exp)
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
                    lTransactions.Add(await UpdateTranferSimulation(transactions));
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
                            lTransactions.Add(await UpdateTranferSplitsSimulation(transactions, splits));
                        }
                    }
                }
                lTransactions.Add(transactions);
            }
            return lTransactions;
        }

        private async Task<Transactions> UpdateTranferSplitsSimulation(Transactions? transactions, Splits splits)
        {
            Transactions tContraria = new()
            {
                Date = transactions.Date,
                Accountid = (await iAccountsService.GetByCategoryId(splits.Categoryid ?? -99))?.Id,
                Account = await iAccountsService.GetByCategoryId(splits.Categoryid ?? -99),
                Personid = transactions.Personid,
                Person = transactions.Person,
                Categoryid = transactions.Account.Categoryid,
                Category = await iCategoriesService.GetById(transactions.Account.Categoryid ?? -99),
                Memo = splits.Memo,
                Tagid = transactions.Tagid,
                AmountIn = splits.AmountOut,
                AmountOut = splits.AmountIn
            };

            return tContraria;
        }


        private async Task<Transactions> UpdateTranferSimulation(Transactions transactions)
        {
            Transactions tContraria = new()
            {
                Date = transactions.Date.RemoveTime(),
                Accountid = (await iAccountsService.GetByCategoryId(transactions.Categoryid ?? -99))?.Id,
                Account = await iAccountsService.GetByCategoryId(transactions.Categoryid ?? -99),
                Personid = transactions.Personid,
                Person = transactions.Person,
                Categoryid = transactions.Account?.Categoryid,
                Category = await iCategoriesService.GetById(transactions.Account?.Categoryid ?? -99),
                Memo = transactions.Memo,
                Tagid = transactions.Tagid,
                Tag = transactions.Tag,
                AmountIn = transactions.AmountOut,
                AmountOut = transactions.AmountIn
            };
            return tContraria;
        }


        private async Task<IEnumerable<ExpirationsReminders>?> GetByTransactionReminderid(int id)
        {
            return await manager.GetByTransactionReminderid(id);
        }

        public async Task DeleteByTransactionReminderid(int id)
        {
            foreach (var expirationsReminder in await GetByTransactionReminderid(id))
            {
                await Delete(expirationsReminder);
            }
        }

        public async Task<DateTime?> GetNextReminder(int id)
        {
            return (await GetByTransactionReminderid(id))?.Where(x => !x.Done.HasValue || !x.Done.Value).Min(y => y.Date);
        }
    }
}
