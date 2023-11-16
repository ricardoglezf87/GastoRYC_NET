using GARCA.Data.Managers;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class VBalancebyCategoryService
    {
        private readonly VBalancebyCategoryManager vBalancebyCategoryManager;

        public VBalancebyCategoryService()
        {
            vBalancebyCategoryManager = new VBalancebyCategoryManager();
        }

        private async Task<IEnumerable<VBalancebyCategory>?> GetAll()
        {
            return await vBalancebyCategoryManager.GetAll();
        }

        private async Task<IEnumerable<VBalancebyCategory>?> GetExpensesbyYearMonth(int month, int year)
        {
            return (await GetAll())?.Where(x => x.CategoriesTypesid == (int)CategoriesTypesService.ECategoriesTypes.Expenses && x.Year == year && x.Month == month).ToHashSet();
        }
    }
}
