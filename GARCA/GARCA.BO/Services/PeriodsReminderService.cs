using GARCA.Utlis.Extensions;
using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class PeriodsRemindersService
    {
        private PeriodsRemindersManager periodsRemindersManager;
        
        public enum EPeriodsReminders : int
        {
            Diary = 1,
            Weekly = 2,
            Monthly = 3,
            Bimonthly = 4,
            Quarterly = 5,
            Bianual = 6,
            Annual = 7
        }

        public PeriodsRemindersService()
        {
            periodsRemindersManager = new PeriodsRemindersManager();
        }

        public HashSet<PeriodsReminders?>? GetAll()
        {
            return periodsRemindersManager.GetAll()?.ToHashSetBo();
        }

        public PeriodsReminders? GetById(int? id)
        {
            return (PeriodsReminders?)periodsRemindersManager.GetById(id);
        }

        public EPeriodsReminders? ToEnum(PeriodsReminders? periodsReminders)
        {
            if (periodsReminders != null)
            {
                switch (periodsReminders.Id)
                {
                    case 1:
                        return EPeriodsReminders.Diary;
                    case 2:
                        return EPeriodsReminders.Weekly;
                    case 3:
                        return EPeriodsReminders.Monthly;
                    case 4:
                        return EPeriodsReminders.Bimonthly;
                    case 5:
                        return EPeriodsReminders.Quarterly;
                    case 6:
                        return EPeriodsReminders.Bianual;
                    case 7:
                        return EPeriodsReminders.Annual;

                }
            }

            return null;
        }

        public DateTime? GetNextDate(DateTime? date, EPeriodsReminders? periodsReminders)
        {
            if (date != null)
            {
                switch (periodsReminders)
                {
                    case EPeriodsReminders.Diary:
                        return date.AddDay();
                    case EPeriodsReminders.Weekly:
                        return date.AddWeek();
                    case EPeriodsReminders.Monthly:
                        return date.AddMonth();
                    case EPeriodsReminders.Bimonthly:
                        return date.AddMonth(2);
                    case EPeriodsReminders.Quarterly:
                        return date.AddMonth(3);
                    case EPeriodsReminders.Bianual:
                        return date.AddMonth(6);
                    case EPeriodsReminders.Annual:
                        return date.AddYear();
                }

            }

            return null;
        }
    }
}
