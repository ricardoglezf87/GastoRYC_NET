using DAOLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

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
            if (v == null) return null;

            return new CategoriesTypes()
            {
                id = v.id,
                description = v.description
            };
        }
    }
}
