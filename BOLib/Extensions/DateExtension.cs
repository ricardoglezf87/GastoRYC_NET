using System;

namespace BOLib.Extensions
{
    public static class DateExtension
    {
        public static String toShortDateString(this DateTime? date)
        {
            return date != null ? date?.Day.ToString("00") + "/" + date?.Month.ToString("00") + "/" + date?.Year.ToString() : "";
        }

        public static DateTime? addDay(this DateTime? date)
        {
            return date != null ? (date?.Date.AddDays(1)) : null;
        }

        public static DateTime? addDay(this DateTime? date, int n)
        {
            return date != null ? (date?.Date.AddDays(n)) : null;
        }

        public static DateTime? addWeek(this DateTime? date)
        {
            return date != null ? (date?.Date.AddDays(7)) : null;
        }

        public static DateTime? addMonth(this DateTime? date)
        {
            return date != null ? (date?.Date.AddMonths(1)) : null;
        }

        public static DateTime? addMonth(this DateTime? date, int amount)
        {
            return date != null ? (date?.Date.AddMonths(amount)) : null;
        }

        public static DateTime? addYear(this DateTime? date)
        {
            return date != null ? (date?.Date.AddYears(1)) : null;
        }

        public static DateTime? removeTime(this DateTime? date)
        {
            return date != null ? new DateTime(date.Value.Year, date.Value.Month, date.Value.Day) : null;
        }

    }
}
