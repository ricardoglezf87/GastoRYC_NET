using DAOLib.Models;
using System.Collections.Generic;
using System.Linq;
using DAOLib.Services;

namespace DAOLib.Managers
{
    public class VBalancebyCategoryManager
    {
        public List<VBalancebyCategoryDAO>? getAll()
        {
            return RYCContextServiceDAO.getInstance()?.BBDD?.vBalancebyCategory?.ToList();
        }
    }
}
