using DAOLib.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace BOLib.Models
{
    public class Tags
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? description { set; get; }

        public static explicit operator Tags(TagsDAO? v)
        {
            return new Tags()
            {
                id = v.id,
                description = v.description
            };
        }
    }
}
