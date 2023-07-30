using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class Persons : ModelBase
    {
        public virtual String? Name { set; get; }
        public virtual int? Categoryid { set; get; }
        public virtual Categories? Category { set; get; }

        internal PersonsDAO ToDao()
        {
            return new PersonsDAO
            {
                id = Id,
                name = Name,
                categoryid = Categoryid,
                category = null
            };
        }

        public static explicit operator Persons?(PersonsDAO? v)
        {
            return v == null
                ? null
                : new Persons
                {
                    Id = v.id,
                    Name = v.name,
                    Categoryid = v.categoryid,
                    Category = v.category != null ? (Categories?)v.category : null
                };
        }
    }
}
