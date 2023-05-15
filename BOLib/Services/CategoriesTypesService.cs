using BOLib.Extensions;
using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;

namespace BOLib.Services
{
    public class CategoriesTypesService
    {
        private readonly CategoriesTypesManager categoriesTypesManager;

        public enum eCategoriesTypes : int
        {
            Expenses = 1,
            Incomes = 2,
            Transfers = 3,
            Specials = 4
        }

        public CategoriesTypesService()
        {
            categoriesTypesManager = InstanceBase<CategoriesTypesManager>.Instance;
        }

        public List<CategoriesTypes>? getAll()
        {
            return categoriesTypesManager.getAll()?.toListBO();
        }

        public List<CategoriesTypes>? getAllFilterTransfer()
        {
            return categoriesTypesManager.getAllFilterTransfer()?.toListBO();
        }

        public CategoriesTypes? getByID(int? id)
        {
            return (CategoriesTypes)categoriesTypesManager.getByID(id);
        }

    }
}
