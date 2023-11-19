using Dommel;
using GARCA.Data.Services;
using GARCA.Models;
using Google.Apis.Sheets.v4.Data;
using System.Configuration;
using System.Linq.Expressions;

using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class TransactionsRemindersManager : ManagerBase<TransactionsReminders>
    {
        public async override Task<IEnumerable<TransactionsReminders>?> GetAll()
        {
            return await iRycContextService.getConnection().GetAllAsync<TransactionsReminders, 
                PeriodsReminders, Accounts, Categories, TransactionsStatus, Persons, Tags, TransactionsReminders>();
        }
    }
}
