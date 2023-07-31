using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("ExpirationsReminders")]
    public class ExpirationsRemindersDAO : ModelBaseDAO
    {
        [Column("date")]
        public virtual DateTime? date { set; get; }

        [Column("transactionsRemindersid")]
        public virtual int? transactionsRemindersid { set; get; }

        [Column("transactionsReminders")]
        public virtual TransactionsRemindersDAO? transactionsReminders { set; get; }

        [Column("done")]
        public virtual bool? done { set; get; }
    }
}
