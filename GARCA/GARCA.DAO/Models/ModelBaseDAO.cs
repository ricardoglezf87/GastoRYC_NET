using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    public class ModelBaseDAO
    {
        [Key]
        [Column("id")]
        public virtual int id { set; get; }
    }
}
