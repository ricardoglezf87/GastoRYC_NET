using GARCA.Data.Managers;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class CategoriesService
    {

        private readonly CategoriesManager categoriesManager;

        public enum ESpecialCategories
        {
            Cierre = -2,
            Split = -1,
            WithoutCategory = 0
        }

        public CategoriesService()
        {
            categoriesManager = new CategoriesManager();
        }

        public HashSet<Categories?>? GetAll()
        {
            return categoriesManager.GetAll()?.ToHashSet();
        }

        public HashSet<Categories?>? GetAllWithoutSpecialTransfer()
        {
            return categoriesManager.GetAllWithoutSpecialTransfer()?.ToHashSet();
        }

        public Categories? GetById(int? id)
        {
            return categoriesManager.GetById(id);
        }

        public void Update(Categories categories)
        {
            categoriesManager.Update(categories);
        }

        public void Delete(Categories categories)
        {
            categoriesManager.Delete(categories);
        }

        public int GetNextId()
        {
            return categoriesManager.GetNextId();
        }
    }
}
