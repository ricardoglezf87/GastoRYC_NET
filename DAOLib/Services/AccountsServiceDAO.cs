using DAOLib.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DAOLib.Services
{
    public class AccountsServiceDAO
    {        
        public List<AccountsDAO>? getAll()
        {
            return RYCContextServiceDAO.getInstance().BBDD.accounts?.ToList();
        }

        public List<AccountsDAO>? getAllOrderByAccountsTypesId()
        {
            return RYCContextServiceDAO.getInstance().BBDD.accounts?.OrderBy(x => x.accountsTypesid).ToList();
        }

        public List<AccountsDAO>? getAllOpened()
        {
            return RYCContextServiceDAO.getInstance().BBDD.accounts?.Where(x => !x.closed.HasValue || !x.closed.Value).ToList();
        }

        public AccountsDAO? getByID(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.accounts?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(AccountsDAO accounts)
        {
            RYCContextServiceDAO.getInstance().BBDD.Update(accounts);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

        public void delete(AccountsDAO accounts)
        {
            RYCContextServiceDAO.getInstance().BBDD.Remove(accounts);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }
    }
}
