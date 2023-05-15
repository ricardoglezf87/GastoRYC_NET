using System.ComponentModel.DataAnnotations;

namespace DAOLib.Models
{
    public class ModelBaseDAO
    {
        [Key]
        public virtual int id { set; get; }
    }
}
