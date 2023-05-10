using DAOLib.Models;
using DAOLib.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DAOLib.Managers
{
    public class AccountsManagerDAO : IManagerDAO<AccountsDAO>
    {        
        public List<AccountsDAO>? getAllOrderByAccountsTypesId()
        {
            return RYCContextServiceDAO.getInstance().BBDD.accounts?.OrderBy(x => x.accountsTypesid).ToList();
        }

        public List<AccountsDAO>? getAllOpened()
        {
            return RYCContextServiceDAO.getInstance().BBDD.accounts?.Where(x => !x.closed.HasValue || !x.closed.Value).ToList();
        }
    }
}
