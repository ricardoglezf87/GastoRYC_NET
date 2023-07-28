using GARCA.BO.Extensions;

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

        public HashSet<VBalancebyCategory?>? getAll()
        {
            return vBalancebyCategoryManager.getAll()?.toHashSetBO();
        }

        public HashSet<VBalancebyCategory?>? getExpensesbyYearMonth(int month, int year)
        {
            return getAll()?.Where(x => x.categoriesTypesid == (int)CategoriesTypesService.eCategoriesTypes.Expenses && x.year == year && x.month == month).ToHashSet();
        }

        public async Task<HashSet<VBalancebyCategory?>?> getExpensesbyYearMonthAsync(int month, int year)
        {
            return await Task.Run(() => getExpensesbyYearMonth(month, year));
        }
    }
}
