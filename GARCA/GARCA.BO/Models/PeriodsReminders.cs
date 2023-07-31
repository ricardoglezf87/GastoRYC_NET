using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class PeriodsReminders : ModelBase
    {
        public virtual String? Description { set; get; }

        public static explicit operator PeriodsReminders?(PeriodsRemindersDao? v)
        {
            return v == null
                ? null
                : new PeriodsReminders
                {
                    Id = v.Id,
                    Description = v.Description
                };
        }

    }
}
