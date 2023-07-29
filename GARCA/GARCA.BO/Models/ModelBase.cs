using System;
using System.ComponentModel.DataAnnotations;

namespace GARCA.BO.Models
{
    public class ModelBase : IComparable
    {
        [Key]
        public virtual int id { set; get; }

        public int CompareTo(object? obj)
        {
            if (obj == null) return 0;

            return this.id.CompareTo(((ModelBase)obj).id);
        }
    }
}
