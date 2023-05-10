using DAOLib.Models;
using System.Collections.Generic;
using System.Linq;
using DAOLib.Services;

namespace DAOLib.Managers
{
    public class VBalancebyCategoryManagerDAO
    {
        public List<VBalancebyCategoryDAO>? getAll()
        {
            return RYCContextServiceDAO.getInstance()?.BBDD?.vBalancebyCategory?.ToList();
        }

        public List<VBalancebyCategoryDAO>? getbyYearMonth(int month, int year)
        {
            return getAll()?.Where(x => x.year == year && x.month == month).ToList();
        }

        public List<VBalancebyCategoryDAO>? getExpensesbyYearMonth(int month, int year)
        {
            return getAll()?.Where(x => x.categoriesTypesid == (int)CategoriesTypesManagerDAO.eCategoriesTypes.Expenses && x.year == year && x.month == month).ToList();
        }
    }
}
