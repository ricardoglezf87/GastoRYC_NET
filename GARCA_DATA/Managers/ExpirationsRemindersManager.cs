
using Dapper;
using Dommel;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class ExpirationsRemindersManager : ManagerBase<ExpirationsReminders>
    {
        public override async Task<IEnumerable<ExpirationsReminders>?> GetAll()
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.GetAllAsync<ExpirationsReminders, TransactionsReminders, ExpirationsReminders>(
                (expirationsReminders, transactionsReminders) =>
                {
                    expirationsReminders.TransactionsReminders = transactionsReminders;
                    expirationsReminders.TransactionsReminders.Categories = iCategoriesService.GetById(transactionsReminders.CategoriesId ?? -99).Result;
                    return expirationsReminders;
                });
            }
        }

        public async Task<bool> ExistsExpiration(TransactionsReminders transactionsReminder, DateTime date)
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.SelectAsync<ExpirationsReminders>(
                x => x.TransactionsRemindersId == transactionsReminder.Id && x.Date == date) != null;
            }
        }

        public async Task<DateTime?> MaxExpiration(TransactionsReminders transactionsReminder)
        {
            using (var connection = iRycContextService.getConnection())
            {
                return (await connection.QueryAsync<DateTime>(
                    $"select max(date) from ExpirationsReminders where transactionsRemindersId={transactionsReminder.Id}")
                    ).FirstOrDefault();
            }
        }

        public async Task<IEnumerable<ExpirationsReminders>?> GetByTransactionReminderid(int id)
        {
            using (var connection = iRycContextService.getConnection())
            {
                return await connection.FromAsync<ExpirationsReminders>(sql=>sql
                    .Where(x => x.TransactionsRemindersId == id)
                    .Select());
            }
        }
    }
}
