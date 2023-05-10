using DAOLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Managers
{
    public class DateCalendarServiceDAO
    {
        private readonly DateTime initDate = new DateTime(2001, 01, 01);

        public List<DateCalendarDAO>? getAll()
        {
            return RYCContextServiceDAO.getInstance().BBDD.dateCalendar?.ToList();
        }

        public DateCalendarDAO? getByID(DateTime? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.dateCalendar?.FirstOrDefault(x => id.Equals(x.date));
        }

        public void fillCalendar()
        {
            DateTime ini = initDate;
            while (ini < DateTime.Now.AddYears(1))
            {
                if (getByID(ini) == null)
                {
                    DateCalendarDAO date = new DateCalendarDAO()
                    {
                        date = ini,
                        day = ini.Day,
                        month = ini.Month,
                        year = ini.Year
                    };

                    RYCContextServiceDAO.getInstance().BBDD.dateCalendar.Add(date);
                }

                ini = ini.AddDays(1);
            }

            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

        public void update(DateCalendarDAO dateCalendar)
        {
            RYCContextServiceDAO.getInstance().BBDD.Update(dateCalendar);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

        public void delete(DateCalendarDAO dateCalendar)
        {
            RYCContextServiceDAO.getInstance().BBDD.Remove(dateCalendar);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }
    }
}
