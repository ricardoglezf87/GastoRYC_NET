using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("DateCalendar")]
    public class DateCalendar : ModelBase
    {
        [Column("date")]
        public virtual DateTime? Date { set; get; }

        [Column("day")]
        public virtual int? Day { set; get; }

        [Column("month")]
        public virtual int? Month { set; get; }

        [Column("year")]
        public virtual int? Year { set; get; }
    }
}
