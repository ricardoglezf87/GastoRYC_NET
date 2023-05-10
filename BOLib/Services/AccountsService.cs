using AutoMapper;
using BOLib.Helpers;
using BOLib.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BOLib.Services
{
    public class AccountsService
    {        
        public List<Accounts>? getAll()
        {
            return MapperConfig.InitializeAutomapper().Map <List<Accounts>>(RYCContextService.getInstance().BBDD.accounts?.ToList());
        }

        public List<Accounts>? getAllOrderByAccountsTypesId()
        {
            return MapperConfig.InitializeAutomapper().Map<List<Accounts>>(RYCContextService.getInstance().BBDD.accounts?.OrderBy(x => x.accountsTypesid).ToList());
        }

        public List<Accounts>? getAllOpened()
        {
            return MapperConfig.InitializeAutomapper().Map<List<Accounts>>(RYCContextService.getInstance().BBDD.accounts?.Where(x => !x.closed.HasValue || !x.closed.Value).ToList());
        }

        public Accounts? getByID(int? id)
        {
            return MapperConfig.InitializeAutomapper().Map<Accounts>(RYCContextService.getInstance().BBDD.accounts?.FirstOrDefault(x => id.Equals(x.id)));
        }

        public void update(Accounts accounts)
        {
            RYCContextService.getInstance().BBDD.Update(accounts);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(Accounts accounts)
        {
            RYCContextService.getInstance().BBDD.Remove(accounts);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }
    }
}
