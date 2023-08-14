using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class Tags : ModelBase
    {
        public virtual String? Description { set; get; }

        internal TagsDao ToDao()
        {
            return new TagsDao
            {
                Id = Id,
                Description = Description
            };
        }

        public static explicit operator Tags?(TagsDao? v)
        {
            return v == null
                ? null
                : new Tags
                {
                    Id = v.Id,
                    Description = v.Description
                };
        }
    }
}
