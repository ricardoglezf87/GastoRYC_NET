using DAOLib.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DAOLib.Services
{
    public class AccountsServiceDAO : IServiceDAO<AccountsDAO>
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
