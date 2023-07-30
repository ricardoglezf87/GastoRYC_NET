using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using System;

namespace GARCA.DAO.Managers
{
    public class DateCalendarManager
    {

        public DateCalendarDAO? GetById(DateTime? id)
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryGeneral<DateCalendarDAO>();
                return repository.GetById(id);
            }
        }

        public void Add(DateCalendarDAO? dateCalendar)
        {
            if (dateCalendar != null)
            {
                using (var unitOfWork = new UnitOfWork(new RycContext()))
                {
                    var repository = unitOfWork.GetRepositoryGeneral<DateCalendarDAO>();
                    repository.Add(dateCalendar);
                    repository.SaveChanges();
                }
            }
        }

        public void SaveChanges()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var repository = unitOfWork.GetRepositoryGeneral<DateCalendarDAO>();
                repository.SaveChanges();
            }
        }
    }
}
