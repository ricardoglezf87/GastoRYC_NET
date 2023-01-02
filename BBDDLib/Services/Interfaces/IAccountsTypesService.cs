using BBDDLib.Models;
using System.Collections.Generic;

namespace GastosRYC.BBDDLib.Services
{
    public interface IAccountsTypesService
    {

        public List<AccountsTypes>? getAll();

        public AccountsTypes? getByID(int? id);

    }
}
