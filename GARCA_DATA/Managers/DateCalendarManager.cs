

using Dommel;
using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class DateCalendarManager : ManagerBase<DateCalendar>
    {
        public async Task<DateCalendar?> GetByDate(DateTime date)
        {
            var obj = await iRycContextService.getConnection().SelectAsync<DateCalendar>(x => x.Date == date);
            return obj == null || !obj.Any() ? null : obj.First();
        }
    }
}
