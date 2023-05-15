using DAOLib.Models;
using DAOLib.Services;
using System.Collections.Generic;
using System.Linq;

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
