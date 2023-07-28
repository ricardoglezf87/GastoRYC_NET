using GARCA.BO.Extensions;
using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class CategoriesTypesService
    {
        private readonly CategoriesTypesManager categoriesTypesManager;
        private static CategoriesTypesService? _instance;
        private static readonly object _lock = new();

        public static CategoriesTypesService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new CategoriesTypesService();
                    }
                }
                return _instance;
            }
        }

        public enum eCategoriesTypes : int
        {
            Expenses = 1,
            Incomes = 2,
            Transfers = 3,
            Specials = 4
        }

        private CategoriesTypesService()
        {
            categoriesTypesManager = new();
        }

        public List<CategoriesTypes?>? getAllWithoutSpecialTransfer()
        {
            return categoriesTypesManager.getAllWithoutSpecialTransfer()?.toListBO();
        }

        public CategoriesTypes? getByID(int? id)
        {
            return (CategoriesTypes?)categoriesTypesManager.getByID(id);
        }

    }
}
