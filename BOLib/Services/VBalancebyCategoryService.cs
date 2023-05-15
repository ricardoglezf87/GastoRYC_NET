using BOLib.Extensions;

using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
{
    public class VBalancebyCategoryService
    {
        private readonly VBalancebyCategoryManager vBalancebyCategoryManager;

        public VBalancebyCategoryService()
        {
            vBalancebyCategoryManager = InstanceBase<VBalancebyCategoryManager>.Instance;
        }

        public List<VBalancebyCategory>? getAll()
        {
            return vBalancebyCategoryManager.getAll()?.toListBO();
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
