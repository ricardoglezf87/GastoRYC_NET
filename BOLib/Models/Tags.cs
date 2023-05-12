using DAOLib.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace BOLib.Models
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

        public static explicit operator Tags(TagsDAO? v)
        {
            if (v == null) return null;

            return new Tags()
            {
                id = v.id,
                description = v.description
            };
        }
    }
}
