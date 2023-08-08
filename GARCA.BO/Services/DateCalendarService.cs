using GARCA.BO.Extensions;

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
    }
}
