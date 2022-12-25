using BBDDLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using GastosRYC.Extensions;

namespace GastosRYC.BBDDLib.Services
{
    public class PeriodsRemindersService
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


        public List<PeriodsReminders>? getAll()
        {
            return RYCContextService.getInstance().BBDD.periodsReminders?.ToList();
        }

        public PeriodsReminders? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.periodsReminders?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(PeriodsReminders periodsReminders)
        {
            RYCContextService.getInstance().BBDD.Update(periodsReminders);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(PeriodsReminders periodsReminders)
        {
            RYCContextService.getInstance().BBDD.Remove(periodsReminders);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public DateTime? getNextDate(DateTime? date, ePeriodsReminders periodsReminders)
        {
            if(date != null)
            {
                switch(periodsReminders)
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
