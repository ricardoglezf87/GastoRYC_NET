using GARCA.Utlis.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GARCA.BO.Services
{
    public class VBalancebyCategoryService
    {
        private readonly VBalancebyCategoryManager vBalancebyCategoryManager;
       
        public VBalancebyCategoryService()
        {
            vBalancebyCategoryManager = new VBalancebyCategoryManager();
        }

        private HashSet<VBalancebyCategory?>? getAll()
        {
            return vBalancebyCategoryManager.getAll()?.toHashSetBO();
        }

        private HashSet<VBalancebyCategory?>? getExpensesbyYearMonth(int month, int year)
        {
            return getAll()?.Where(x => x.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Expenses && x.year == year && x.month == month).ToHashSet();
        }

        public async Task<HashSet<VBalancebyCategory?>?> getExpensesbyYearMonthAsync(int month, int year)
        {
            return await Task.Run(() => getExpensesbyYearMonth(month, year));
        }
    }
}
