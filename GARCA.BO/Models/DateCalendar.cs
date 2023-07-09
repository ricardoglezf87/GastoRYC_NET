using GARCA.DAO.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace GARCA.BO.Models
{
    public class DateCalendar
    {
        [Key]
        public virtual DateTime date { set; get; }
        public virtual int? day { set; get; }
        public virtual int? month { set; get; }
        public virtual int? year { set; get; }

        internal DateCalendarDAO toDAO()
        {
            return new DateCalendarDAO()
            {
                date = this.date,
                day = this.day,
                month = this.month,
                year = this.year
            };
        }

        public static explicit operator DateCalendar?(DateCalendarDAO? v)
        {
            return v == null
                ? null
                : new DateCalendar()
                {
                    date = v.date,
                    day = v.day,
                    month = v.month,
                    year = v.year
                };
        }
    }
}
