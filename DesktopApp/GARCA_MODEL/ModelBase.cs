using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    public class ModelBase : IComparable
    {
        [Key]
        [Column("id")]
        public virtual int Id { set; get; }

        public virtual int CompareTo(object? obj)
        {
            return obj == null ? 0 : Id.CompareTo(((ModelBase)obj).Id);
        }
    }
}
