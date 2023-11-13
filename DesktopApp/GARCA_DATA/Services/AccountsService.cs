using Dapper;
using GARCA.Data.Managers;
using GARCA.Models;
using GARCA_DATA.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;
using static GARCA.Data.IOC.DependencyConfig;


namespace GARCA.Data.Services
{
    public class AccountsService : IServiceCache<Accounts>
    {
        protected override IEnumerable<Accounts>? GetAllCache()
        {
            return iRycContextService.getConnection().Query<Accounts,AccountsTypes,Accounts>(
                @"
                    select * 
                    from Accounts
                        inner join AccountsTypes on AccountsTypes.Id = Accounts.accountsTypesid
                "
                ,(a, at) =>
            {
                a.AccountsTypes = at;
                return a;
            }).AsEnumerable();

        }

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
