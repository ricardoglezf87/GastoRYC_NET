namespace GARCA.Utils.Extensions
{
    public static class DateExtension
    {
        public static string ToShortDateString(this DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("dd/MM/yyyy") : "";
        }

        public static DateTime? AddDay(this DateTime? date)
        {
            return date?.Date.AddDays(1);
        }

        public static DateTime? AddDay(this DateTime? date, int n)
        {
            return date?.Date.AddDays(n);
        }

        public static DateTime? AddWeek(this DateTime? date)
        {
            return date?.Date.AddDays(7);
        }

        public static DateTime? AddMonth(this DateTime? date)
        {
            return date?.Date.AddMonths(1);
        }

        public static DateTime? AddMonth(this DateTime? date, int amount)
        {
            return date?.Date.AddMonths(amount);
        }

        public static DateTime? AddYear(this DateTime? date)
        {
            return date?.Date.AddYears(1);
        }

        public static DateTime? RemoveTime(this DateTime? date)
        {
            return date.HasValue ? DateTime.SpecifyKind(date.Value.Date, DateTimeKind.Utc) : null;
        }

    }
}
