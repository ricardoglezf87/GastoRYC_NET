using System.ComponentModel.DataAnnotations;

namespace GARCA.BO.Models
{
    public class ModelBase
    {
        [Key]
        public virtual int id { set; get; }
    }
}
