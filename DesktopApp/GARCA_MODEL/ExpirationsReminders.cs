using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("ExpirationsReminders")]
    public class ExpirationsReminders : ModelBase
    {
        [Column("date")]
        public virtual DateTime? Date { set; get; }

        [Column("transactionsRemindersid")]
        public virtual int? TransactionsRemindersId { set; get; }

        [Column("transactionsReminders")]
        public virtual TransactionsReminders? TransactionsReminders { set; get; }

        [Column("done")]
        public virtual bool? Done { set; get; }

        [NotMapped]
        public virtual string GroupDate => Date != null
                   ? Date < DateTime.Now ? "Vencido" : Date > DateTime.Now.AddMonths(1) ? "Futuro" : Date.Value.Day + "/" + Date.Value.Month
                   : string.Empty;
    }
}
