using System;

namespace GARCA.Utlis.Extensions
{
    public static class DateExtension
    {
        public static String toShortDateString(this DateTime? date)
        {
            return date != null ? date?.Day.ToString("00") + "/" + date?.Month.ToString("00") + "/" + date?.Year : "";
        }

        public static DateTime? addDay(this DateTime? date)
        {
            return  (date?.Date.AddDays(1)) ;
        }

        public static DateTime? addDay(this DateTime? date, int n)
        {
            return date?.Date.AddDays(n);
        }

        public static DateTime? addWeek(this DateTime? date)
        {
            return date?.Date.AddDays(7);
        }

        public static DateTime? addMonth(this DateTime? date)
        {
            return date?.Date.AddMonths(1);
        }

        public static DateTime? addMonth(this DateTime? date, int amount)
        {
            return date?.Date.AddMonths(amount);
        }

        public static DateTime? addYear(this DateTime? date)
        {
            return date?.Date.AddYears(1);
        }

        public static DateTime? removeTime(this DateTime? date)
        {
            return date != null ? new DateTime(date.Value.Year, date.Value.Month, date.Value.Day) : null;
        }

    }
}
