using DAOLib.Models;
using DAOLib.Services;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Services
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
            return getAll()?.Where(x => x.categoriesTypesid == (int)CategoriesTypesServiceDAO.eCategoriesTypes.Expenses && x.year == year && x.month == month).ToList();
        }
    }
}
