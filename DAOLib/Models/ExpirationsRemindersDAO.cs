using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAOLib.Models
{
    [Table("ExpirationsReminders")]
    public class ExpirationsRemindersDAO : IModelDAO
    {
        public virtual DateTime? date { set; get; }

        public virtual int? transactionsRemindersid { set; get; }

        public virtual TransactionsRemindersDAO? transactionsReminders { set; get; }

        public virtual bool? done { set; get; }
    }
}
