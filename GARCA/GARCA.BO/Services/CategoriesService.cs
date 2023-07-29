using GARCA.Utlis.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class CategoriesService
    {

        private readonly CategoriesManager categoriesManager;
        
        public enum eSpecialCategories : int
        {
            Split = -1,
            WithoutCategory = 0
        }

        public CategoriesService()
        {
            categoriesManager = new CategoriesManager();
        }

        public HashSet<Categories?>? getAll()
        {
            return categoriesManager.getAll()?.toHashSetBO();
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
