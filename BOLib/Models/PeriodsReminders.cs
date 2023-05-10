using DAOLib.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace BOLib.Models
{
    public class PeriodsReminders
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? description { set; get; }

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
