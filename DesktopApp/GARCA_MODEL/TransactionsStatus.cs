using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("TransactionsStatus")]
    public class TransactionsStatus : ModelBase<Int32>
    {
        [Column("description")]
        public virtual String? Description { set; get; }
    }
}
