using BOLib.Helpers;
using BOLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
{
    public class DateCalendarService
    {
        private readonly DateTime initDate = new DateTime(2001, 01, 01);

        public List<DateCalendar>? getAll()
        {
            return MapperConfig.InitializeAutomapper().Map<List<DateCalendar>>(RYCContextService.getInstance().BBDD.dateCalendar?.ToList());
        }

        public DateCalendar? getByID(DateTime? id)
        {
            return MapperConfig.InitializeAutomapper().Map<DateCalendar>(RYCContextService.getInstance().BBDD.dateCalendar?.FirstOrDefault(x => id.Equals(x.date)));
        }

        public void fillCalendar()
        {
            DateTime ini = initDate;
            while (ini < DateTime.Now.AddYears(1))
            {
                if (getByID(ini) == null)
                {
                    //TODO:Revisar esto que no es correcto
                    DAOLib.Models.DateCalendar date = new DAOLib.Models.DateCalendar()
                    {
                        date = ini,
                        day = ini.Day,
                        month = ini.Month,
                        year = ini.Year
                    };
                    
                        RYCContextService.getInstance().BBDD.dateCalendar.Add(date);
                }

                ini = ini.AddDays(1);
            }

            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void update(DateCalendar dateCalendar)
        {
            RYCContextService.getInstance().BBDD.Update(dateCalendar);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(DateCalendar dateCalendar)
        {
            RYCContextService.getInstance().BBDD.Remove(dateCalendar);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }
    }
}
