using GARCA.DAO.Models;

namespace GARCA.BO.Models
{
    public class ExpirationsReminders : ModelBase
    {
        public virtual DateTime? Date { set; get; }

        public virtual int? TransactionsRemindersid { set; get; }

        public virtual TransactionsReminders? TransactionsReminders { set; get; }

        public virtual bool? Done { set; get; }

        public virtual String GroupDate => Date != null
                    ? Date < DateTime.Now ? "Vencido" : Date > DateTime.Now.AddMonths(1) ? "Futuro" : Date.Value.Day + "/" + Date.Value.Month
                    : String.Empty;

        internal ExpirationsRemindersDao ToDao()
        {
            return new ExpirationsRemindersDao
            {
                Id = Id,
                Date = Date,
                TransactionsReminders = null,
                TransactionsRemindersid = TransactionsRemindersid,
                Done = Done
            };
        }

        public static explicit operator ExpirationsReminders?(ExpirationsRemindersDao? v)
        {
            return v == null
                ? null
                : new ExpirationsReminders
                {
                    Id = v.Id,
                    Date = v.Date,
                    TransactionsReminders = v.TransactionsReminders != null ? (TransactionsReminders?)v.TransactionsReminders : null,
                    TransactionsRemindersid = v.TransactionsRemindersid,
                    Done = v.Done
                };
        }
    }
}
