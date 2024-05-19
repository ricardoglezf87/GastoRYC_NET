using GARCA.wsData.Repositories;
using GARCA.Models;
using GARCA.Utils.Extensions;
using Google.Apis.Sheets.v4.Data;
using static GARCA.Data.IOC.DependencyConfig;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace GARCA.Data.Services
{
    public class ExpirationsRemindersService : ServiceBase<ExpirationsRemindersRepository, ExpirationsReminders>
    {      
        private async Task<bool> ExistsExpiration(TransactionsReminders transactionsReminder, DateTime date)
        {
            return await repository.ExistsExpiration(transactionsReminder, date);
        }

        public async Task<DateTime?> MaxExpiration(TransactionsReminders transactionsReminder)
        {
            return await repository.MaxExpiration(transactionsReminder);
        }

        public async Task<IEnumerable<ExpirationsReminders>?> GetAllPending()
        {
            return await repository.GetAllPending();
        }

        public async Task GenerateAllExpirations()
        {
            foreach (var transactionsReminders in await iTransactionsRemindersService.GetAll())
            {
                await GenerateExpiration(transactionsReminders);
            }
        }

        public async Task DoAutoregister()
        {
            foreach (var exp in await repository.GetAllExpirationReadyToAutoregister())
            {                
                await RegisterTransactionfromReminder(exp.Id);
                exp.Done = true;
                await Save(exp);
            }
        }

        private async Task GenerateExpiration(TransactionsReminders transactionsReminders)
        {
            if (transactionsReminders != null)
            {
                var date = await MaxExpiration(transactionsReminders) ?? transactionsReminders.Date;

                while (date <= DateTime.Now.AddYears(1))
                {
                    if (!await ExistsExpiration(transactionsReminders, date.Value))
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
                var expirationsReminders = await GetById(id.Value);

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
                    transactions = await iTransactionsService.Save(transactions);

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
                        await iSplitsService.Save(splits);
                    }

                    return transactions;
                }
            }

            return null;
        }
            
        private async Task<IEnumerable<ExpirationsReminders>?> GetByTransactionReminderid(int id)
        {
            return await repository.GetByTransactionReminderid(id);
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
