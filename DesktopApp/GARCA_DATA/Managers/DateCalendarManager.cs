using GARCA.DAO.Repositories;
using GARCA.Models;

namespace GARCA.Data.Managers
{
    public class DateCalendarManager
    {

        public DateCalendar? GetById(DateTime? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryGeneral<DateCalendar>();
                return repository.GetById(id);
            }
        }

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
