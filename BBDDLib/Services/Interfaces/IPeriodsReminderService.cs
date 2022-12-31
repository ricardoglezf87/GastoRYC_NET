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
    public interface IPeriodsRemindersService
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


        public List<PeriodsReminders>? getAll();

        public PeriodsReminders? getByID(int? id);

        public void update(PeriodsReminders periodsReminders);

        public void delete(PeriodsReminders periodsReminders);

        public ePeriodsReminders? toEnum(PeriodsReminders? periodsReminders);   
        
        public DateTime? getNextDate(DateTime? date, ePeriodsReminders? periodsReminders);

    }
}
