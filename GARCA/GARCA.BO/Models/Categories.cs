using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class Categories : ModelBase
    {
        public virtual String? Description { set; get; }

        public virtual int? CategoriesTypesid { set; get; }

        public virtual CategoriesTypes? CategoriesTypes { set; get; }

        internal CategoriesDao ToDao()
        {
            return new CategoriesDao
            {
                Id = Id,
                Description = Description,
                CategoriesTypesid = CategoriesTypesid,
                CategoriesTypes = null
            };
        }

        public static explicit operator Categories?(CategoriesDao? v)
        {
            return v == null
                ? null
                : new Categories
                {
                    Id = v.Id,
                    Description = v.Description,
                    CategoriesTypesid = v.CategoriesTypesid,
                    CategoriesTypes = v.CategoriesTypes != null ? (CategoriesTypes?)v.CategoriesTypes : null
                };
        }
    }
}
