using System.ComponentModel.DataAnnotations;

namespace GARCA.DAO.Models
{
    public class ModelBaseDAO
    {
        [Key]
        public virtual int id { set; get; }
    }
}
