using GARCA.Data.Managers;
using GARCA.Models;
using GARCA.Utils.Extensions;
using static GARCA.Data.IOC.DependencyConfig;


namespace GARCA.Data.Services
{
    public class ExpirationsRemindersService : ServiceBase<ExpirationsRemindersManager, ExpirationsReminders>
    {
        private async Task<IEnumerable<ExpirationsReminders>?> GetAllWithGeneration()
        {
            await GenerationAllExpirations();
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
                await GenerationExpirations(transactionsReminders);
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
                await Save(exp);
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
                        expirationsReminders.TransactionsRemindersId = transactionsReminders.Id;
                        expirationsReminders.TransactionsReminders = transactionsReminders;
                        expirationsReminders.Date = date;
                        await Save(expirationsReminders);
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

                if (expirationsReminders == null)
                {
                    return null;
                }

                expirationsReminders.TransactionsReminders = await iTransactionsRemindersService.
                    GetById(expirationsReminders.TransactionsRemindersId ?? -99);

                if (expirationsReminders.TransactionsReminders != null)
                {
                    Transactions? transactions = new();
                    transactions.Date = expirationsReminders.Date;
                    transactions.AccountsId = expirationsReminders.TransactionsReminders.AccountsId;
                    transactions.PersonsId = expirationsReminders.TransactionsReminders.PersonsId;
                    transactions.CategoriesId = expirationsReminders.TransactionsReminders.CategoriesId;
                    transactions.Memo = expirationsReminders.TransactionsReminders.Memo;
                    transactions.AmountIn = expirationsReminders.TransactionsReminders.AmountIn;
                    transactions.AmountOut = expirationsReminders.TransactionsReminders.AmountOut;
                    transactions.TagsId = expirationsReminders.TransactionsReminders.TagsId;
                    transactions.TransactionsStatusId = (int)TransactionsStatusService.ETransactionsTypes.Pending;
                    transactions = await iTransactionsService.SaveChanges(transactions);

                    foreach (var splitsReminders in
                        await iSplitsRemindersService.GetbyTransactionid(expirationsReminders.TransactionsReminders.Id))
                    {
                        Splits splits = new();
                        splits.TransactionsId = transactions.Id;
                        splits.CategoriesId = splitsReminders.CategoriesId;
                        splits.Memo = splitsReminders.Memo;
                        splits.AmountIn = splitsReminders.AmountIn;
                        splits.AmountOut = splitsReminders.AmountOut;
                        splits.TagsId = splitsReminders.TagsId;

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
                    AccountsId = expirationsReminders.TransactionsReminders.AccountsId,
                    Accounts = expirationsReminders.TransactionsReminders.Accounts,
                    PersonsId = expirationsReminders.TransactionsReminders.PersonsId,
                    Persons = expirationsReminders.TransactionsReminders.Persons,
                    CategoriesId = expirationsReminders.TransactionsReminders.CategoriesId,
                    Categories = expirationsReminders.TransactionsReminders.Categories,
                    Memo = expirationsReminders.TransactionsReminders.Memo,
                    AmountIn = expirationsReminders.TransactionsReminders.AmountIn ?? 0,
                    AmountOut = expirationsReminders.TransactionsReminders.AmountOut ?? 0,
                    Tags = expirationsReminders.TransactionsReminders.Tags,
                    TagsId = expirationsReminders.TransactionsReminders.TagsId
                };

                if (await iCategoriesService.IsTranfer(transactions.CategoriesId ?? -99))
                {
                    lTransactions.Add(await UpdateTranferSimulation(transactions));
                }

                if (expirationsReminders.TransactionsReminders.Splits != null)
                {

                    foreach (var splitsReminders in expirationsReminders.TransactionsReminders.Splits)
                    {
                        Splits splits = new()
                        {
                            TransactionsId = transactions.Id,
                            CategoriesId = splitsReminders.CategoriesId,
                            Categories = splitsReminders.Categories,
                            Memo = splitsReminders.Memo,
                            AmountIn = splitsReminders.AmountIn ?? 0,
                            AmountOut = splitsReminders.AmountOut ?? 0,
                            TagsId = splitsReminders.TagsId
                        };

                        if (await iCategoriesService.IsTranfer(splits.CategoriesId ?? -99))
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
                AccountsId = (await iAccountsService.GetByCategoryId(splits.CategoriesId ?? -99))?.Id,
                Accounts = await iAccountsService.GetByCategoryId(splits.CategoriesId ?? -99),
                PersonsId = transactions.PersonsId,
                Persons = transactions.Persons,
                CategoriesId = transactions.Accounts.Categoryid,
                Categories = await iCategoriesService.GetById(transactions.Accounts.Categoryid ?? -99),
                Memo = splits.Memo,
                TagsId = transactions.TagsId,
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
                AccountsId = (await iAccountsService.GetByCategoryId(transactions.CategoriesId ?? -99))?.Id,
                Accounts = await iAccountsService.GetByCategoryId(transactions.CategoriesId ?? -99),
                PersonsId = transactions.PersonsId,
                Persons = transactions.Persons,
                CategoriesId = transactions.Accounts?.Categoryid,
                Categories = await iCategoriesService.GetById(transactions.Accounts?.Categoryid ?? -99),
                Memo = transactions.Memo,
                TagsId = transactions.TagsId,
                Tags = transactions.Tags,
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
