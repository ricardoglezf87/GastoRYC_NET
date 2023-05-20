using DAOLib.Migrations;
using DAOLib.Models;
using DAOLib.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Managers
{
    public class DateCalendarManager
    {
        private readonly DateTime initDate = new(2001, 01, 01);

        public List<DateCalendarDAO>? getAll()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryGeneral<DateCalendarDAO>();
                return repository.GetAll();
            }
        }

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

        public void update(DateCalendarDAO dateCalendar)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryGeneral<DateCalendarDAO>();
                repository.Update(dateCalendar);
                repository.saveChanges();
            }
        }

        public void delete(DateCalendarDAO dateCalendar)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryGeneral<DateCalendarDAO>();
                repository.Delete(dateCalendar);
                repository.saveChanges();
            }
        }
    }
}
