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
    public class PeriodsReminderService
    {
        public enum ePeriodsReminder : int
        {
            Diary = 1,
            Weekly = 2,
            Monthly = 3,
            Bimonthly = 4,
            Quarterly = 5,
            Bianual = 6,
            Annual = 7
        }


        public List<PeriodsReminder>? getAll()
        {
            return RYCContextService.getInstance().BBDD.periodsReminder?.ToList();
        }

        public PeriodsReminder? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.periodsReminder?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(PeriodsReminder periodsReminder)
        {
            RYCContextService.getInstance().BBDD.Update(periodsReminder);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(PeriodsReminder periodsReminder)
        {
            RYCContextService.getInstance().BBDD.Remove(periodsReminder);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public DateTime? getNextDate(DateTime? date, ePeriodsReminder periodsReminder)
        {
            if(date != null)
            {
                switch(periodsReminder)
                {
                    case ePeriodsReminder.Diary:
                        return date.addDay();                        
                    case ePeriodsReminder.Weekly:
                        return date.addWeek();                        
                    case ePeriodsReminder.Monthly:
                        return date.addMonth();                        
                    case ePeriodsReminder.Bimonthly:
                        return date.addMonth(2);
                    case ePeriodsReminder.Quarterly:
                        return date.addMonth(3);
                    case ePeriodsReminder.Bianual:
                        return date.addMonth(6);
                    case ePeriodsReminder.Annual:
                        return date.addYear();
                }
                
            }

            return null;            
        }

    }
}
