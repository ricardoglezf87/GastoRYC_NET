using DAOLib.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace BOLib.Models
{
    public class PeriodsReminders : ModelBase
    { 
        public virtual String? description { set; get; }

        internal PeriodsRemindersDAO toDAO()
        {
            return new PeriodsRemindersDAO()
            {
                id = this.id,
                description = this.description
            };
        }

        public static explicit operator PeriodsReminders(PeriodsRemindersDAO? v)
        {
            return new PeriodsReminders()
            {
                id = v.id,
                description = v.description
            };
        }

    }
}
