using BBDDLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class AccountsService
    {

        public List<Accounts>? getAll()
        {
            return RYCContextService.getInstance().BBDD.accounts?.ToList();
        }

        public List<Accounts>? getAllOrderByAccountsTypesId()
        {
            return RYCContextService.getInstance().BBDD.accounts?.OrderBy(x=>x.accountsTypesid).ToList();
        }

        public Accounts? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.accounts?.FirstOrDefault(x => id.Equals(x.id));
        }

    }
}
