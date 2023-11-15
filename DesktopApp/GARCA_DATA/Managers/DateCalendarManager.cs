
using GARCA.Models;

namespace GARCA.Data.Managers
{
    public class DateCalendarManager : ManagerBase<DateCalendar,DateTime>
    {
        public void Add(DateCalendar? dateCalendar)
        {
            if (dateCalendar != null)
            {
                using (var unitOfWork = new UnitOfWork(new RycContext()))
                {
                    var repository = unitOfWork.GetRepositoryGeneral<DateCalendar>();
                    repository.Add(dateCalendar);
                    repository.SaveChanges();
                }
            }
        }

        public void SaveChanges()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryGeneral<DateCalendar>();
                repository.SaveChanges();
            }
        }
    }
}
