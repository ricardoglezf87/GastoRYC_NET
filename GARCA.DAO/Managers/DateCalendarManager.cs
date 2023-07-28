using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using System;
using System.Collections.Generic;

namespace GARCA.DAO.Managers
{
    public class DateCalendarManager
    {

        public DateCalendarDAO? getByID(DateTime? id)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryGeneral<DateCalendarDAO>();
                return repository.GetById(id);
            }
        }

        public void add(DateCalendarDAO? dateCalendar)
        {
            if (dateCalendar != null)
            {
                using (var unitOfWork = new UnitOfWork(new RYCContext()))
                {
                    var repository = unitOfWork.GetRepositoryGeneral<DateCalendarDAO>();
                    repository.Add(dateCalendar);
                    repository.saveChanges();
                }
            }
        }

        public void saveChanges()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryGeneral<DateCalendarDAO>();
                repository.saveChanges();
            }
        }
    }
}
