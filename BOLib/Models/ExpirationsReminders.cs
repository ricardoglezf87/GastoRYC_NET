using DAOLib.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOLib.Models
{
    public class ExpirationsReminders : ModelBase
    {
        public virtual DateTime? date { set; get; }

        public virtual int? transactionsRemindersid { set; get; }

        public virtual TransactionsReminders? transactionsReminders { set; get; }

        public virtual bool? done { set; get; }

        [NotMapped]
        public virtual String? groupDate => date != null
                    ? date < DateTime.Now ? "Vencido" : date > DateTime.Now.AddMonths(1) ? "Futuro" : date.Value.Day + "/" + date.Value.Month
                    : String.Empty;

        internal ExpirationsRemindersDAO toDAO()
        {
            return new ExpirationsRemindersDAO()
            {
                id = this.id,
                date = this.date,
                transactionsReminders = null,
                transactionsRemindersid = this.transactionsRemindersid,
                done = this.done
            };
        }

        public static explicit operator ExpirationsReminders?(ExpirationsRemindersDAO? v)
        {
            return v == null
                ? null
                : new ExpirationsReminders()
                {
                    id = v.id,
                    date = v.date,
                    transactionsReminders = (v.transactionsReminders != null) ? (TransactionsReminders?)v.transactionsReminders : null,
                    transactionsRemindersid = v.transactionsRemindersid,
                    done = v.done
                };
        }
    }
}
