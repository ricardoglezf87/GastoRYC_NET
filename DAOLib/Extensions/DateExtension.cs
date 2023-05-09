using System;

namespace GastosRYC.Extensions
{
    public static class DateExtension
    {
        public static String toShortDateString(this DateTime? date)
        {
            if (date != null)
            {
                return date?.Day.ToString("00") + "/" + date?.Month.ToString("00") + "/" + date?.Year.ToString();
            }
            else
            {
                return "";
            }
        }

        public static DateTime? addDay(this DateTime? date)
        {
            if (date != null)
            {
                return date?.Date.AddDays(1);
            }
            else
            {
                return null;
            }
        }

        public static DateTime? addWeek(this DateTime? date)
        {
            if (date != null)
            {
                return date?.Date.AddDays(7);
            }
            else
            {
                return null;
            }
        }

        public static DateTime? addMonth(this DateTime? date)
        {
            if (date != null)
            {
                return date?.Date.AddMonths(1);
            }
            else
            {
                return null;
            }
        }

        public static DateTime? addMonth(this DateTime? date, int amount)
        {
            if (date != null)
            {
                return date?.Date.AddMonths(amount);
            }
            else
            {
                return null;
            }
        }

        public static DateTime? addYear(this DateTime? date)
        {
            if (date != null)
            {
                return date?.Date.AddYears(1);
            }
            else
            {
                return null;
            }
        }

        public static DateTime? removeTime(this DateTime? date)
        {
            if (date != null)
            {
                return new DateTime(date.Value.Year, date.Value.Month, date.Value.Day);
            }
            else
            {
                return null;
            }
        }

    }
}
