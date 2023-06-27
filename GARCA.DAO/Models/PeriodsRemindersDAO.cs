using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("PeriodsReminders")]
    public class PeriodsRemindersDAO : ModelBaseDAO
    {
        public virtual String? description { set; get; }

    }
}
