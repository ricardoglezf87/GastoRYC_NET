using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("ExpirationsReminders")]
    public class ExpirationsRemindersDao : ModelBaseDao
    {
        [Column("date")]
        public virtual DateTime? Date { set; get; }

        [Column("transactionsRemindersid")]
        public virtual int? TransactionsRemindersid { set; get; }

        [Column("transactionsReminders")]
        public virtual TransactionsRemindersDao? TransactionsReminders { set; get; }

        [Column("done")]
        public virtual bool? Done { set; get; }
    }
}
