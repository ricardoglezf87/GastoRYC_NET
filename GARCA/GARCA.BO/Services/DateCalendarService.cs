using GARCA.Utlis.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System;
using System.Collections.Generic;

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

        private DateCalendar? getByID(DateTime? id)
        {
            return (DateCalendar?)dateCalendarManager.getByID(id);
        }

        public void fillCalendar()
        {
            var ini = initDate;
            while (ini < DateTime.Now.AddYears(1))
            {
                if (getByID(ini) == null)
                {
                    DateCalendar date = new()
                    {
                        date = ini,
                        day = ini.Day,
                        month = ini.Month,
                        year = ini.Year
                    };

                    dateCalendarManager.add(date.toDAO());
                }

                ini = ini.AddDays(1);
            }

            dateCalendarManager.saveChanges();
        }
    }
}
