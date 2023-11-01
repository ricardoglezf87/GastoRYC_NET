using GARCA.Models;
using GARCA.Data.Managers;


namespace GARCA.Data.Services
{
    public class VBalancebyCategoryService
    {
        private readonly VBalancebyCategoryManager vBalancebyCategoryManager;

        public VBalancebyCategoryService()
        {
            vBalancebyCategoryManager = new VBalancebyCategoryManager();
        }

        private HashSet<VBalancebyCategory?>? GetAll()
        {
            return vBalancebyCategoryManager.GetAll()?.ToHashSet();
        }

        private HashSet<VBalancebyCategory?>? GetExpensesbyYearMonth(int month, int year)
        {
            return GetAll()?.Where(x => x.CategoriesTypesid == (int)CategoriesTypesService.ECategoriesTypes.Expenses && x.Year == year && x.Month == month).ToHashSet();
        }

        public async Task<HashSet<VBalancebyCategory?>?> GetExpensesbyYearMonthAsync(int month, int year)
        {
            return await Task.Run(() => GetExpensesbyYearMonth(month, year));
        }
    }
}
