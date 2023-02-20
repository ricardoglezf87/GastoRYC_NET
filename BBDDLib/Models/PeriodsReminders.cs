using System;
using System.ComponentModel.DataAnnotations;

namespace BBDDLib.Models
{
    public class PeriodsReminders
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? description { set; get; }

    }
}
