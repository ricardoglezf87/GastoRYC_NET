using BOLib.Extensions;

using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOLib.Services
{
    public class VBalancebyCategoryService
    {
        private readonly VBalancebyCategoryManager vBalancebyCategoryManager;
        private static VBalancebyCategoryService? _instance;
        private static readonly object _lock = new();

        public static VBalancebyCategoryService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new VBalancebyCategoryService();
                    }
                }
                return _instance;
            }
        }

        private VBalancebyCategoryService()
        {
            vBalancebyCategoryManager = new();
        }

        public List<VBalancebyCategory?>? getAll()
        {
            return vBalancebyCategoryManager.getAll()?.toListBO();
        }

        public List<VBalancebyCategory?>? getbyYearMonth(int month, int year)
        {
            return getAll()?.Where(x => x.year == year && x.month == month).ToList();
        }

        public List<VBalancebyCategory?>? getExpensesbyYearMonth(int month, int year)
        {
            return getAll()?.Where(x => x.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Expenses && x.year == year && x.month == month).ToList();
        }

        public async Task<List<VBalancebyCategory?>?> getExpensesbyYearMonthAsync(int month, int year)
        {
            return await Task.Run(() => getExpensesbyYearMonth(month, year));
        }
    }
}
