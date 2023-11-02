using GARCA.Data.Managers;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class CategoriesTypesService
    {
        private readonly CategoriesTypesManager categoriesTypesManager;

        public enum ECategoriesTypes
        {
            Expenses = 1,
            Incomes = 2,
            Transfers = 3,
            Specials = 4
        }

        public CategoriesTypesService()
        {
            categoriesTypesManager = new CategoriesTypesManager();
        }

        public HashSet<CategoriesTypes?>? GetAllWithoutSpecialTransfer()
        {
            return categoriesTypesManager.GetAllWithoutSpecialTransfer()?.ToHashSet();
        }

        public CategoriesTypes? GetById(int? id)
        {
            return categoriesTypesManager.GetById(id);
        }

    }
}
