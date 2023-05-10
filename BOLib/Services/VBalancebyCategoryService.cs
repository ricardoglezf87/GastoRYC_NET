using BOLib.Helpers;
using BOLib.Models;
using BOLib.Services;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
{
    public class VBalancebyCategoryService
    {
        public List<VBalancebyCategory>? getAll()
        {
            return MapperConfig.InitializeAutomapper().Map<List<VBalancebyCategory>>(RYCContextService.getInstance()?.BBDD?.vBalancebyCategory?.ToList());
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
