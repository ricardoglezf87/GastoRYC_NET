using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class Categories : ModelBase
    {
        public virtual String? description { set; get; }

        public virtual int? categoriesTypesid { set; get; }

        public virtual CategoriesTypes? categoriesTypes { set; get; }

        internal CategoriesDAO toDAO()
        {
            return new CategoriesDAO
            {
                id = this.id,
                description = this.description,
                categoriesTypesid = this.categoriesTypesid,
                categoriesTypes = null
            };
        }

        public static explicit operator Categories?(CategoriesDAO? v)
        {
            return v == null
                ? null
                : new Categories
                {
                    id = v.id,
                    description = v.description,
                    categoriesTypesid = v.categoriesTypesid,
                    categoriesTypes = (v.categoriesTypes != null) ? (CategoriesTypes?)v.categoriesTypes : null
                };
        }
    }
}
