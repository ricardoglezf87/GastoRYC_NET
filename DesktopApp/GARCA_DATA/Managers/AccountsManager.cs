using Dapper;
using Dommel;
using GARCA.Data.Managers;
using GARCA.Data.Services;
using GARCA.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA_DATA.Managers
{
    public class AccountsManager : ManagerBase<Accounts>
    {
        public async override Task<IEnumerable<Accounts>?> GetAll()
        {
            return await iRycContextService.getConnection().GetAllAsync<Accounts, AccountsTypes, Accounts>();
        }        
    }
}
