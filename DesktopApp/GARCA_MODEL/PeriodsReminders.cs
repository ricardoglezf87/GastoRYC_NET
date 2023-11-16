using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("PeriodsReminders")]
    public class PeriodsReminders : ModelBase<Int32>
    {
        [Column("description")]
        public virtual String? Description { set; get; }

    }
}
