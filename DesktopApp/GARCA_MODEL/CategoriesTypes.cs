using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("CategoriesTypes")]
    public class CategoriesTypes : ModelBase<Int32>
    {
        [Column("description")]
        public virtual String? Description { set; get; }
    }
}
