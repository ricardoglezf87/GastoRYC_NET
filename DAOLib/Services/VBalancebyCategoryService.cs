using DAOLib.Models;
using DAOLib.Services;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Services
{
    public class VBalancebyCategoryService
    {
        public List<VBalancebyCategory>? getAll()
        {
            return RYCContextService.getInstance()?.BBDD?.vBalancebyCategory?.ToList();
        }

        public List<VBalancebyCategory>? getbyYearMonth(int month, int year)
        {
            return getAll()?.Where(x => x.year == year && x.month == month).ToList();
        }

        public List<VBalancebyCategory>? getExpensesbyYearMonth(int month, int year)
        {
            return getAll()?.Where(x => x.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Expenses && x.year == year && x.month == month).ToList();
        }
    }
}
