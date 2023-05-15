using DAOLib.Models;
using DAOLib.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Managers
{
    public class DateCalendarManager
    {
        private readonly DateTime initDate = new(2001, 01, 01);

        public List<DateCalendarDAO>? getAll()
        {
            return RYCContextServiceDAO.getInstance().BBDD.dateCalendar?.ToList();
        }

        public DateCalendarDAO? getByID(DateTime? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.dateCalendar?.FirstOrDefault(x => id.Equals(x.date));
        }

        public void add(DateCalendarDAO? obj)
        {
            if (obj != null)
            {
                RYCContextServiceDAO.getInstance().BBDD.Add(obj);
            }
        }

        public void saveChanges()
        {
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

        public void update(DateCalendarDAO dateCalendar)
        {
            RYCContextServiceDAO.getInstance().BBDD.Update(dateCalendar);
            saveChanges();
        }

        public void delete(DateCalendarDAO dateCalendar)
        {
            RYCContextServiceDAO.getInstance().BBDD.Remove(dateCalendar);
            saveChanges();
        }
    }
}
