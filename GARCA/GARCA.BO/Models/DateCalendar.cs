using GARCA.DAO.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace GARCA.BO.Models
{
    public class DateCalendar
    {
        [Key]
        public virtual DateTime Date { set; get; }
        public virtual int? Day { set; get; }
        public virtual int? Month { set; get; }
        public virtual int? Year { set; get; }

        internal DateCalendarDAO ToDao()
        {
            return new DateCalendarDAO
            {
                date = Date,
                day = Day,
                month = Month,
                year = Year
            };
        }

        public static explicit operator DateCalendar?(DateCalendarDAO? v)
        {
            return v == null
                ? null
                : new DateCalendar
                {
                    Date = v.date,
                    Day = v.day,
                    Month = v.month,
                    Year = v.year
                };
        }
    }
}
