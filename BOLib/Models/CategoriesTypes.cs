using DAOLib.Models;
using System;

namespace BOLib.Models
{
    public class CategoriesTypes : ModelBase
    {
        public virtual String? description { set; get; }

        internal CategoriesTypesDAO toDAO()
        {
            return new CategoriesTypesDAO()
            {
                id = this.id,
                description = this.description
            };
        }

        public static explicit operator CategoriesTypes?(CategoriesTypesDAO? v)
        {
            return v == null
                ? null
                : new CategoriesTypes()
                {
                    id = v.id,
                    description = v.description
                };
        }
    }
}
