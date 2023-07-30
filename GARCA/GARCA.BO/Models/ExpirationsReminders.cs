using GARCA.DAO.Models;
using System;

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

        internal ExpirationsRemindersDAO ToDao()
        {
            return new ExpirationsRemindersDAO
            {
                id = Id,
                date = Date,
                transactionsReminders = null,
                transactionsRemindersid = TransactionsRemindersid,
                done = Done
            };
        }

        public static explicit operator ExpirationsReminders?(ExpirationsRemindersDAO? v)
        {
            return v == null
                ? null
                : new ExpirationsReminders
                {
                    Id = v.id,
                    Date = v.date,
                    TransactionsReminders = v.transactionsReminders != null ? (TransactionsReminders?)v.transactionsReminders : null,
                    TransactionsRemindersid = v.transactionsRemindersid,
                    Done = v.done
                };
        }
    }
}
