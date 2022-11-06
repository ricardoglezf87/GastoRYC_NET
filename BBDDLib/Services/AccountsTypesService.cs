using BBDDLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class AccountsTypesService
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
