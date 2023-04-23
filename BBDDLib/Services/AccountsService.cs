using BBDDLib.Models;
using BBDDLib.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace GastosRYC.BBDDLib.Services
{
    public class AccountsService : IAccountsService
    {

        public List<Accounts>? getAll()
        {
            return RYCContextService.getInstance().BBDD.accounts?.ToList();
        }

        public List<Accounts>? getAllOrderByAccountsTypesId()
        {
            return RYCContextService.getInstance().BBDD.accounts?.OrderBy(x => x.accountsTypesid).ToList();
        }

        public List<Accounts>? getAllOpened()
        {
            return RYCContextService.getInstance().BBDD.accounts?.Where(x => !x.closed.HasValue || !x.closed.Value).ToList();
        }

        public Accounts? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.accounts?.FirstOrDefault(x => id.Equals(x.id));
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
