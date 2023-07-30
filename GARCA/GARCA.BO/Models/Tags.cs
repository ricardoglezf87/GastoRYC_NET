using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class Tags : ModelBase
    {
        public virtual String? Description { set; get; }

        internal TagsDAO ToDao()
        {
            return new TagsDAO
            {
                id = this.Id,
                description = this.Description
            };
        }

        public static explicit operator Tags?(TagsDAO? v)
        {
            return v == null
                ? null
                : new Tags
                {
                    Id = v.id,
                    Description = v.description
                };
        }
    }
}
