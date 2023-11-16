using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("Tags")]
    public class Tags : ModelBase<Int32>
    {
        [Column("description")]
        public virtual String? Description { set; get; }
    }
}
