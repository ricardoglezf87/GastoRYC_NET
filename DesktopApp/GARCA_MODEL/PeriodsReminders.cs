using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("PeriodsReminders")]
    public class PeriodsReminders : ModelBase
    {
        [Column("description")]
        public virtual String? Description { set; get; }

    }
}
