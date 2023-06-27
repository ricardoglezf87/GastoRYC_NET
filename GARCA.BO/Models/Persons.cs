using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class Persons : ModelBase
    {
        public virtual String? name { set; get; }
        public virtual int? categoryid { set; get; }
        public virtual Categories? category { set; get; }

        internal PersonsDAO toDAO()
        {
            return new PersonsDAO()
            {
                id = this.id,
                name = this.name,
                categoryid = this.categoryid,
                category = null
            };
        }

        public static explicit operator Persons?(PersonsDAO? v)
        {
            return v == null
                ? null
                : new Persons()
                {
                    id = v.id,
                    name = v.name,
                    categoryid = v.categoryid,
                    category = (v.category != null) ? (Categories?)v.category : null
                };
        }
    }
}
