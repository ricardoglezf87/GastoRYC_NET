

using Dommel;
using GARCA.Models;


namespace GARCA.wsData.Managers
{
    public class DateCalendarManager : ManagerBase<DateCalendar>
    {
        public async Task<DateCalendar?> GetByDate(DateTime date)
        {
            var obj = await dbContext.OpenConnection().SelectAsync<DateCalendar>(x => x.Date == date);
            return obj == null || !obj.Any() ? null : obj.First();
        }
    }
}
