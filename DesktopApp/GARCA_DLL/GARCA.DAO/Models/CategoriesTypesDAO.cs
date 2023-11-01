using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("CategoriesTypes")]
    public class CategoriesTypesDao : ModelBaseDao
    {
        [Column("description")]
        public virtual String? Description { set; get; }
    }
}
