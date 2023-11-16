

using static GARCA.Data.IOC.DependencyConfig;
using GARCA.Data.Services;
using GARCA.Models;
using Dommel;
using Dapper;

namespace GARCA.Data.Managers
{
    public class DateCalendarManager : ManagerBase<DateCalendar>
    {
        public async Task<DateCalendar?> GetByDate(DateTime date)
        {
            return (await iRycContextService.getConnection().SelectAsync<DateCalendar>(x=>x.Date == date))?.First();
        }
    }
}
