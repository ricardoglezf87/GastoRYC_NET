using DAOLib.Models;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BOLib.Models
{
    public class Categories : ModelBase
    {
        public virtual String? description { set; get; }

        public virtual int? categoriesTypesid { set; get; }

        public virtual CategoriesTypes? categoriesTypes { set; get; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Accounts? accounts { set; get; }

        internal CategoriesDAO toDAO()
        {
            return new CategoriesDAO()
            {
                id = this.id,
                description = this.description,
                categoriesTypesid = this.categoriesTypesid,
                categoriesTypes = this.categoriesTypes?.toDAO()
            };
        }

        public static explicit operator Categories(CategoriesDAO? v)
        {
            return new Categories()
            {
                id = v.id,
                description = v.description,
                categoriesTypesid = v.categoriesTypesid,
                categoriesTypes = (v.categoriesTypes != null) ? (CategoriesTypes)v.categoriesTypes : null
            };
        }
    }
}
