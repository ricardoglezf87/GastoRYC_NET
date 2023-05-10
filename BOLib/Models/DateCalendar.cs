using System;
using System.ComponentModel.DataAnnotations;

namespace BOLib.Models
{
    public class DateCalendar
    {
        [Key]
        public virtual DateTime date { set; get; }
        public virtual int? day { set; get; }
        public virtual int? month { set; get; }
        public virtual int? year { set; get; }
    }
}
