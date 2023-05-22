using System.ComponentModel.DataAnnotations;

namespace BOLib.Models
{
    public class ModelBase
    {
        [Key]
        public virtual int id { set; get; }
    }
}
