using GARCA.DAO.Models;
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

        internal DateCalendarDao ToDao()
        {
            return new DateCalendarDao
            {
                Date = Date,
                Day = Day,
                Month = Month,
                Year = Year
            };
        }

        public static explicit operator DateCalendar?(DateCalendarDao? v)
        {
            return v == null
                ? null
                : new DateCalendar
                {
                    Date = v.Date,
                    Day = v.Day,
                    Month = v.Month,
                    Year = v.Year
                };
        }
    }
}
