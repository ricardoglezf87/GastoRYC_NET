using BBDDLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace GastosRYC.BBDDLib.Services
{
    public class AccountsTypesService : IAccountsTypesService
    {

        public List<AccountsTypes>? getAll()
        {
            return RYCContextService.getInstance().BBDD.accountsTypes?.ToList();
        }

        public AccountsTypes? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.accountsTypes?.FirstOrDefault(x => id.Equals(x.id));
        }

    }
}
