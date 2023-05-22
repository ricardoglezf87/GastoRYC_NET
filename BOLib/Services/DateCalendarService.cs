using BOLib.Extensions;

using BOLib.Models;
using DAOLib.Managers;
using System;
using System.Collections.Generic;

namespace BOLib.Services
{
    public class DateCalendarService
    {
        private readonly DateCalendarManager dateCalendarManager;
        private readonly DateTime initDate;
        private static DateCalendarService? _instance;
        private static readonly object _lock = new();

        public static DateCalendarService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new DateCalendarService();
                    }
                }
                return _instance;
            }
        }

        private DateCalendarService()
        {
            dateCalendarManager = new();
            initDate = new DateTime(2001, 01, 01);
        }

        public List<DateCalendar?>? getAll()
        {
            return dateCalendarManager.getAll()?.toListBO();
        }

        public DateCalendar? getByID(DateTime? id)
        {
            return (DateCalendar?)dateCalendarManager.getByID(id);
        }

        public void fillCalendar()
        {
            DateTime ini = initDate;
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
