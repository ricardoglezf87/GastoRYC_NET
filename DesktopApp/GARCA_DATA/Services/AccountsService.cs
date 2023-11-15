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
    public class AccountsService : ServiceBase<AccountsManager,Accounts, Int32>
    {
        public HashSet<Accounts>? GetAllOpened()
        {
            return GetAll()?.Where(x=> x.Closed is null || x.Closed is false).ToHashSet();
        }

        public Accounts? GetByCategoryId(int? id)
        {
            return GetAll()?.First(x => x.Categoryid == id);
        }

        public Decimal GetBalanceByAccount(int? id)
        {
            return iTransactionsService.GetByAccount(id)?.Sum(x => x.Amount) ?? 0;
        }
    }
}
