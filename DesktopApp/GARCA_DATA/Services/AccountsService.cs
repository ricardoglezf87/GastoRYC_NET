using Dapper;
using GARCA.Data.Managers;
using GARCA.Models;
using GARCA.Data.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;
using static GARCA.Data.IOC.DependencyConfig;
using GARCA_DATA.Managers;

namespace GARCA.Data.Services
{
    public class AccountsService : ServiceBase<AccountsManager,Accounts>
    {
        public async Task<IEnumerable<Accounts>?> GetAllOpened()
        {
            return (await GetAll())?.Where(x=> x.Closed is null || x.Closed is false).ToHashSet();
        }

        public async Task<Accounts?> GetByCategoryId(int id)
        {
            return (await GetAll())?.First(x => x.Categoryid == id);
        }

        public async Task<Decimal> GetBalanceByAccount(int? id)
        {
            return (await iTransactionsService.GetByAccount(id))?.Sum(x => x.Amount) ?? 0;
        }
    }
}
