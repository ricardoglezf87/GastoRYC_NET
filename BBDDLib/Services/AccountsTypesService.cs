using BBDDLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class AccountsTypesStatus
    {

        public List<AccountsTypes>? getAll()
        {
            return RYCContextService.Instance.BBDD.accountsTypes?.ToList();
        }

        public AccountsTypes? getByID(int? id)
        {
            return RYCContextService.Instance.BBDD.accountsTypes?.FirstOrDefault(x => id.Equals(x.id));
        }

    }
}
