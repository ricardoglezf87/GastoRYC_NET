
using Dapper;
using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Repositories
{
    public class ExpirationsRemindersRepository : RepositoryBase<ExpirationsReminders>
    {       

        public async Task<bool> ExistsExpiration(TransactionsReminders transactionsReminder, DateTime date)
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.SelectAsync<ExpirationsReminders>(
                x => x.TransactionsRemindersId == transactionsReminder.Id && x.Date == date) != null;
            }
        }

        public async Task<IEnumerable<ExpirationsReminders>?> GetAllExpirationReadyToAutoregister()
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.QueryAsync<ExpirationsReminders>(
                    @"select er.* 
                    from ExpirationsReminders er 
	                    join TransactionsReminders tr on tr.id = er.transactionsRemindersid 
		                    and (done is null or done=false) and er.date <= now()
		                    and tr.autoRegister = true"
                    );
            }
        }

        public async Task<IEnumerable<ExpirationsReminders>?> GetAllPending()
        {
            DateTime futuro = DateTime.Now.AddMonths(1);

            using (var connection = dbContext.OpenConnection())
            {
                IEnumerable<ExpirationsReminders> list = await connection.SelectAsync<ExpirationsReminders>(
                    x => (x.Done == null || x.Done != true) && x.Date <= futuro);

                foreach (var item in list)
                {
                    item.TransactionsReminders = await connection
                        .GetAsync<TransactionsReminders, Categories, Persons, TransactionsReminders>(
                            item.TransactionsRemindersId ?? -99);
                }

                return list;
            }
        }

        public async Task<DateTime?> MaxExpiration(TransactionsReminders transactionsReminder)
        {
            using (var connection = dbContext.OpenConnection())
            {
                return (await connection.QueryAsync<DateTime>(
                    $"select max(date) from ExpirationsReminders where transactionsRemindersId={transactionsReminder.Id}")
                    ).FirstOrDefault();
            }
        }

        public async Task<IEnumerable<ExpirationsReminders>?> GetByTransactionReminderid(int id)
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.FromAsync<ExpirationsReminders>(sql => sql
                    .Where(x => x.TransactionsRemindersId == id)
                    .Select());
            }
        }

        public override async Task<IEnumerable<ExpirationsReminders>?> GetAll()
        {
            using (var connection = dbContext.OpenConnection())
            {
                return await connection.GetAllAsync<ExpirationsReminders, TransactionsReminders, ExpirationsReminders>(
                (expirationsReminders, transactionsReminders) =>
                {
                    expirationsReminders.TransactionsReminders = transactionsReminders;
                    expirationsReminders.TransactionsReminders.Categories = connection.Get<Categories>(transactionsReminders.CategoriesId ?? -99);
                    return expirationsReminders;
                });
            }
        }
    }
}
