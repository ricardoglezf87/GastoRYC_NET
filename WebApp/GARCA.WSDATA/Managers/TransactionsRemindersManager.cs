using Dommel;
using GARCA.Models;



namespace GARCA.wsData.Managers
{
    public class TransactionsRemindersManager : ManagerBase<TransactionsReminders>
    {
        public override async Task<IEnumerable<TransactionsReminders>?> GetAll()
        {
            return await dbContext.OpenConnection().GetAllAsync<TransactionsReminders,
                PeriodsReminders, Accounts, Categories, TransactionsStatus, Persons, Tags, TransactionsReminders>();
        }
    }
}
