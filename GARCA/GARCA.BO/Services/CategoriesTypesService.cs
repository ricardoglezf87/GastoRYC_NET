using GARCA.Utlis.Extensions;
using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class CategoriesTypesService
    {
        private readonly CategoriesTypesManager categoriesTypesManager;

        public enum ECategoriesTypes : int
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
            return categoriesTypesManager.GetAllWithoutSpecialTransfer()?.ToHashSetBo();
        }

        public CategoriesTypes? GetById(int? id)
        {
            return (CategoriesTypes?)categoriesTypesManager.GetById(id);
        }

    }
}
