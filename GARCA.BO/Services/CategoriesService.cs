using GARCA.BO.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class CategoriesService
    {

        private readonly CategoriesManager categoriesManager;
        private static CategoriesService? _instance;
        private static readonly object _lock = new();

        public static CategoriesService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new CategoriesService();
                    }
                }
                return _instance;
            }
        }

        public enum eSpecialCategories : int
        {
            Split = -1,
            WithoutCategory = 0
        }

        private CategoriesService()
        {
            categoriesManager = new();
        }

        public List<Categories?>? getAll()
        {
            return categoriesManager.getAll()?.toListBO();
        }

        public List<Categories?>? getAllWithoutSpecialTransfer()
        {
            return categoriesManager?.getAllWithoutSpecialTransfer()?.toListBO();
        }

        public Categories? getByID(int? id)
        {
            return (Categories?)categoriesManager.getByID(id);
        }

        public void update(Categories categories)
        {
            categoriesManager.update(categories?.toDAO());
        }

        public void delete(Categories categories)
        {
            categoriesManager.delete(categories?.toDAO());
        }

        public int getNextID()
        {
            return categoriesManager.getNextID();
        }
    }
}
