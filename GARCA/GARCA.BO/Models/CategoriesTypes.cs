using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class CategoriesTypes : ModelBase
    {
        public virtual String? description { set; get; }
        
        public static explicit operator CategoriesTypes?(CategoriesTypesDAO? v)
        {
            return v == null
                ? null
                : new CategoriesTypes
                {
                    id = v.id,
                    description = v.description
                };
        }
    }
}
