using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class PeriodsReminders : ModelBase
    {
        public virtual String? description { set; get; }

        internal PeriodsRemindersDAO toDAO()
        {
            return new PeriodsRemindersDAO
            {
                id = this.id,
                description = this.description
            };
        }

        public static explicit operator PeriodsReminders?(PeriodsRemindersDAO? v)
        {
            return v == null
                ? null
                : new PeriodsReminders
                {
                    id = v.id,
                    description = v.description
                };
        }

    }
}
