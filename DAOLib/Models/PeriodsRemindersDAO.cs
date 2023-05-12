using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAOLib.Models
{
    [Table("PeriodsReminders")]
    public class PeriodsRemindersDAO : ModelBaseDAO
    {
        public virtual String? description { set; get; }

    }
}
