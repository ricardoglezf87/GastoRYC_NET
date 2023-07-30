using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class Categories : ModelBase
    {
        public virtual String? Description { set; get; }

        public virtual int? CategoriesTypesid { set; get; }

        public virtual CategoriesTypes? CategoriesTypes { set; get; }

        internal CategoriesDAO ToDao()
        {
            return new CategoriesDAO
            {
                id = Id,
                description = Description,
                categoriesTypesid = CategoriesTypesid,
                categoriesTypes = null
            };
        }

        public static explicit operator Categories?(CategoriesDAO? v)
        {
            return v == null
                ? null
                : new Categories
                {
                    Id = v.id,
                    Description = v.description,
                    CategoriesTypesid = v.categoriesTypesid,
                    CategoriesTypes = v.categoriesTypes != null ? (CategoriesTypes?)v.categoriesTypes : null
                };
        }
    }
}
