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
            return RYCContextService.Instance.BBDD.accounts?.ToList();
        }

        public Accounts? getByID(long? id)
        {
            return RYCContextService.Instance.BBDD.accounts?.FirstOrDefault(x => id.Equals(x.id));
        }

    }
}
