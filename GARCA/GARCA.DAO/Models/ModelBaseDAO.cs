using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    public class ModelBaseDao
    {
        [Key]
        [Column("id")]
        public virtual int Id { set; get; }
    }
}
