using GARCA.BO.Extensions;
using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class PeriodsRemindersService
    {
        public PeriodsRemindersManager periodsRemindersManager;
        private static PeriodsRemindersService? _instance;
        private static readonly object _lock = new();

        public static PeriodsRemindersService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new PeriodsRemindersService();
                    }
                }
                return _instance;
            }
        }

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

        private PeriodsRemindersService()
        {
            periodsRemindersManager = new();
        }

        public HashSet<PeriodsReminders?>? getAll()
        {
            return periodsRemindersManager.getAll()?.toHashSetBO();
        }

        public PeriodsReminders? getByID(int? id)
        {
            return (PeriodsReminders?)periodsRemindersManager.getByID(id);
        }

        public ePeriodsReminders? toEnum(PeriodsReminders? periodsReminders)
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

        public DateTime? getNextDate(DateTime? date, ePeriodsReminders? periodsReminders)
        {
            if (date != null)
            {
                switch (periodsReminders)
                {
                    case ePeriodsReminders.Diary:
                        return date.addDay();
                    case ePeriodsReminders.Weekly:
                        return date.addWeek();
                    case ePeriodsReminders.Monthly:
                        return date.addMonth();
                    case ePeriodsReminders.Bimonthly:
                        return date.addMonth(2);
                    case ePeriodsReminders.Quarterly:
                        return date.addMonth(3);
                    case ePeriodsReminders.Bianual:
                        return date.addMonth(6);
                    case ePeriodsReminders.Annual:
                        return date.addYear();
                }

            }

            return null;
        }
    }
}
