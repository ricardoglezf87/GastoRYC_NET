using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("DateCalendar")]
    public class DateCalendar : ModelBase<DateTime>
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

//TODO: Hay que realizar la migracion del campo date por i