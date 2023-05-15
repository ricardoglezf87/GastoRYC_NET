using BOLib.Extensions;

using BOLib.Models;
using DAOLib.Managers;
using DAOLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
{
    public class DateCalendarService
    {
        private readonly DateCalendarManager dateCalendarManager;
        private readonly DateTime initDate;

        public DateCalendarService()
        {
            dateCalendarManager = InstanceBase<DateCalendarManager>.Instance;
            initDate = new DateTime(2001, 01, 01);
        }

        public List<DateCalendar>? getAll()
        {
            return dateCalendarManager.getAll()?.toListBO();
        }

        public DateCalendar? getByID(DateTime? id) => (DateCalendar?)dateCalendarManager.getByID(id);

        public void fillCalendar()
        {
            DateTime ini = initDate;
            while (ini < DateTime.Now.AddYears(1))
            {
                if (getByID(ini) == null)
                {
                    DateCalendar date = new DateCalendar()
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

        public void update(DateCalendar dateCalendar)
        {
            dateCalendarManager.update(dateCalendar.toDAO());
        }

        public void delete(DateCalendar dateCalendar)
        {
            dateCalendarManager.delete(dateCalendar.toDAO());
        }
    }
}
