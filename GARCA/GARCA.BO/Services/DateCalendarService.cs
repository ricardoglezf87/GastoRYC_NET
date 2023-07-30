using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System;

namespace GARCA.BO.Services
{
    public class DateCalendarService
    {
        private readonly DateCalendarManager dateCalendarManager;
        private readonly DateTime initDate;

        public DateCalendarService()
        {
            dateCalendarManager = new DateCalendarManager();
            initDate = new DateTime(2001, 01, 01);
        }

        private DateCalendar? GetById(DateTime? id)
        {
            return (DateCalendar?)dateCalendarManager.GetById(id);
        }

        public void FillCalendar()
        {
            var ini = initDate;
            while (ini < DateTime.Now.AddYears(1))
            {
                if (GetById(ini) == null)
                {
                    DateCalendar date = new()
                    {
                        Date = ini,
                        Day = ini.Day,
                        Month = ini.Month,
                        Year = ini.Year
                    };

                    dateCalendarManager.Add(date.ToDao());
                }

                ini = ini.AddDays(1);
            }

            dateCalendarManager.SaveChanges();
        }
    }
}
