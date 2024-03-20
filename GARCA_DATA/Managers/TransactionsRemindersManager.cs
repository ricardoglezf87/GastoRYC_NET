using Dommel;
using GARCA.Models;

using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class TransactionsRemindersManager : ManagerBase<TransactionsReminders>
    {
        public override async Task<IEnumerable<TransactionsReminders>?> GetAll()
        {
            return await iRycContextService.getConnection().GetAllAsync<TransactionsReminders,
                PeriodsReminders, Accounts, Categories, TransactionsStatus, Persons, Tags, TransactionsReminders>();
        }
    }
}
