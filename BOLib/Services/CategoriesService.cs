using BOLib.Extensions;

using BOLib.Models;
using DAOLib.Managers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
{
    public class CategoriesService
    {

        private readonly CategoriesManager categoriesManager;

        //TDOO:Revisar enums
        public enum eSpecialCategories : int
        {
            Split = -1,
            WithoutCategory = 0
        }

        public CategoriesService()
        {
            categoriesManager = InstanceBase<CategoriesManager>.Instance;
        }

        public List<Categories>? getAll()
        {
            return categoriesManager.getAll()?.toListBO();
        }

        public List<Categories>? getAllFilterTransfer()
        {
            return categoriesManager.getAllFilterTransfer()?.toListBO();
        }

        public Categories? getByID(int? id)
        {
            return (Categories)categoriesManager.getByID(id);
        }

        public void update(Categories categories)
        {
            categoriesManager.delete(categories?.toDAO());
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
