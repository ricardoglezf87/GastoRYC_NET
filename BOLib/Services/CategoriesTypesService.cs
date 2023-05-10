using BOLib.Helpers;
using BOLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
{
    public class CategoriesTypesService
    {
        public enum eCategoriesTypes : int
        {
            Expenses = 1,
            Incomes = 2,
            Transfers = 3,
            Specials = 4
        }

        public List<CategoriesTypes>? getAll()
        {
            return MapperConfig.InitializeAutomapper().Map<List<CategoriesTypes>>(RYCContextService.getInstance().BBDD.categoriesTypes?.ToList());
        }

        public List<CategoriesTypes>? getAllFilterTransfer()
        {
            return MapperConfig.InitializeAutomapper().Map<List<CategoriesTypes>>(RYCContextService.getInstance().BBDD.categoriesTypes?
                .Where(x => !x.id.Equals((int)eCategoriesTypes.Transfers) &&
                !x.id.Equals((int)eCategoriesTypes.Transfers)).ToList());
        }

        public CategoriesTypes? getByID(int? id)
        {
            return MapperConfig.InitializeAutomapper().Map<CategoriesTypes>(RYCContextService.getInstance().BBDD.categoriesTypes?.FirstOrDefault(x => id.Equals(x.id)));
        }

    }
}
