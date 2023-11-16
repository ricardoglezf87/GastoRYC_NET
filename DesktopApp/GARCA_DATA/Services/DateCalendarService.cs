using GARCA.Data.Managers;
using GARCA.Models;

namespace GARCA.Data.Services
{
    public class DateCalendarService : ServiceBase<DateCalendarManager, DateCalendar>
    {
        private readonly DateCalendarManager dateCalendarManager;
        private readonly DateTime initDate;

        public DateCalendarService()
        {
            dateCalendarManager = new DateCalendarManager();
            initDate = new DateTime(2001, 01, 01, 0, 0, 0, DateTimeKind.Utc); ;
        }

        public async Task FillCalendar()
        {
            var ini = initDate;
            while (ini < DateTime.Now.AddYears(1))
            {
                if (GetById(ini) == null)
                {
                    DateCalendar date = new()
                    {
                        Id = ini,
                        Day = ini.Day,
                        Month = ini.Month,
                        Year = ini.Year
                    };

                    await dateCalendarManager.Insert(date);
                }

                ini = ini.AddDays(1);
            }
        }
    }
}
