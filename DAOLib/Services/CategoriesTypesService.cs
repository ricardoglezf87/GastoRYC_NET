using BBDDLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace GastosRYC.BBDDLib.Services
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
            return RYCContextService.getInstance().BBDD.categoriesTypes?.ToList();
        }

        public List<CategoriesTypes>? getAllFilterTransfer()
        {
            return RYCContextService.getInstance().BBDD.categoriesTypes?
                .Where(x => !x.id.Equals((int)eCategoriesTypes.Transfers) &&
                !x.id.Equals((int)eCategoriesTypes.Transfers)).ToList();
        }

        public CategoriesTypes? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.categoriesTypes?.FirstOrDefault(x => id.Equals(x.id));
        }

    }
}
