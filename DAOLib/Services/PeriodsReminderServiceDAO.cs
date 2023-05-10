using DAOLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Services
{
    public class PeriodsRemindersServiceDAO
    {
        public enum ePeriodsReminders : int
        {
            Diary = 1,
            Weekly = 2,
            Monthly = 3,
            Bimonthly = 4,
            Quarterly = 5,
            Bianual = 6,
            Annual = 7
        }

        public List<PeriodsRemindersDAO>? getAll()
        {
            return RYCContextServiceDAO.getInstance().BBDD.periodsReminders?.ToList();
        }

        public PeriodsRemindersDAO? getByID(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.periodsReminders?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(PeriodsRemindersDAO periodsReminders)
        {
            RYCContextServiceDAO.getInstance().BBDD.Update(periodsReminders);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

        public void delete(PeriodsRemindersDAO periodsReminders)
        {
            RYCContextServiceDAO.getInstance().BBDD.Remove(periodsReminders);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

        public ePeriodsReminders? toEnum(PeriodsRemindersDAO? periodsReminders)
        {
            if (periodsReminders != null)
            {
                switch (periodsReminders.id)
                {
                    case 1:
                        return ePeriodsReminders.Diary;
                    case 2:
                        return ePeriodsReminders.Weekly;
                    case 3:
                        return ePeriodsReminders.Monthly;
                    case 4:
                        return ePeriodsReminders.Bimonthly;
                    case 5:
                        return ePeriodsReminders.Quarterly;
                    case 6:
                        return ePeriodsReminders.Bianual;
                    case 7:
                        return ePeriodsReminders.Annual;
                }
            }

            return null;
        }      
    }
}
