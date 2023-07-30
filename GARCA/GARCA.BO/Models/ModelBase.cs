using System;
using System.ComponentModel.DataAnnotations;

namespace GARCA.BO.Models
{
    public class ModelBase : IComparable
    {
        [Key]
        public virtual int id { set; get; }

        public virtual int CompareTo(object? obj)
        {
            return obj == null ? 0 : this.id.CompareTo(((ModelBase)obj).id);
        }
    }
}
