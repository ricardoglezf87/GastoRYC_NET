using BBDDLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public interface IAccountsTypesService
    {

        public List<AccountsTypes>? getAll();

        public AccountsTypes? getByID(int? id);

    }
}
