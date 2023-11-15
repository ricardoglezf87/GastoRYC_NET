using Dapper;
using GARCA.Data.Managers;
using GARCA.Data.Services;
using GARCA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA_DATA.Managers
{
    public class AccountsManager : ManagerBase<Accounts, Int32>
    {
        private string GetGeneralQuery()
        {
            return @"
                    select * 
                    from Accounts
                        inner join AccountsTypes on AccountsTypes.Id = Accounts.accountsTypesid
                    ";
        }

        public async override Task<IEnumerable<Accounts>?> GetAll()
        {
            return await iRycContextService.getConnection().QueryAsync<Accounts, AccountsTypes, Accounts>(
                GetGeneralQuery()
                , (a, at) =>
                {
                    a.AccountsTypes = at;
                    return a;
                });
        }
    }
}
