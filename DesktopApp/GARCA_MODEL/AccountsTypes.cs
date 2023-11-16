using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("AccountsTypes")]
    public class AccountsTypes : ModelBase<Int32>
    {
        [Column("description")]
        public virtual String? Description { set; get; }
    }
}
