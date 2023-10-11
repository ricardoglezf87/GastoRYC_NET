using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

namespace GARCA.DAO.Managers
{
    public class DateCalendarManager
    {

        public DateCalendarDao? GetById(DateTime? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryGeneral<DateCalendarDao>();
                return repository.GetById(id);
            }
        }

        public void Add(DateCalendarDao? dateCalendar)
        {
            if (dateCalendar != null)
            {
                using (var unitOfWork = new UnitOfWork(new RycContext()))
                {
                    var repository = unitOfWork.GetRepositoryGeneral<DateCalendarDao>();
                    repository.Add(dateCalendar);
                    repository.SaveChanges();
                }
            }
        }

        public void SaveChanges()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryGeneral<DateCalendarDao>();
                repository.SaveChanges();
            }
        }
    }
}
