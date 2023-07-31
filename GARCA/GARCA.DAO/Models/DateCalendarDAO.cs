using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("DateCalendar")]
    public class DateCalendarDAO
    {
        [Key]
        [Column("date")]
        public virtual DateTime date { set; get; }

        [Column("day")]
        public virtual int? day { set; get; }

        [Column("month")]
        public virtual int? month { set; get; }

        [Column("year")]
        public virtual int? year { set; get; }
    }
}
