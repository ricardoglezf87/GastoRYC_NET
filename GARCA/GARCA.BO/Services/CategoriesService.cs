using GARCA.BO.Models;
using GARCA.DAO.Managers;
using GARCA.Utlis.Extensions;
using System.Collections.Generic;

namespace GARCA.BO.Services
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
            return categoriesManager.GetAll()?.ToHashSetBo();
        }

        public HashSet<Categories?>? GetAllWithoutSpecialTransfer()
        {
            return categoriesManager.GetAllWithoutSpecialTransfer()?.ToHashSetBo();
        }

        public Categories? GetById(int? id)
        {
            return (Categories?)categoriesManager.GetById(id);
        }

        public void Update(Categories categories)
        {
            categoriesManager.Update(categories.ToDao());
        }

        public void Delete(Categories categories)
        {
            categoriesManager.Delete(categories.ToDao());
        }

        public int GetNextId()
        {
            return categoriesManager.GetNextId();
        }
    }
}
