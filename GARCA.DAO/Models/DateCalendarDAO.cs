using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("DateCalendar")]
    public class DateCalendarDAO
    {
        [Key]
        public virtual DateTime date { set; get; }
        public virtual int? day { set; get; }
        public virtual int? month { set; get; }
        public virtual int? year { set; get; }
    }
}
