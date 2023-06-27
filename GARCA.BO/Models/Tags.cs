using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class Tags : ModelBase
    {
        public virtual String? description { set; get; }

        internal TagsDAO toDAO()
        {
            return new TagsDAO()
            {
                id = this.id,
                description = this.description
            };
        }

        public static explicit operator Tags?(TagsDAO? v)
        {
            return v == null
                ? null
                : new Tags()
                {
                    id = v.id,
                    description = v.description
                };
        }
    }
}
