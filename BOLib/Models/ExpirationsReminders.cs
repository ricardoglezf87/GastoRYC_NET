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
        public virtual String? groupDate
        {
            get
            {
                String group = String.Empty;

                return date != null
                    ? date < DateTime.Now ? "Vencido" : date > DateTime.Now.AddMonths(1) ? "Futuro" : date.Value.Day + "/" + date.Value.Month
                    : group;
            }
        }

        internal ExpirationsRemindersDAO toDAO()
        {
            return new ExpirationsRemindersDAO()
            {
                date = this.date,
                transactionsReminders = this.transactionsReminders?.toDAO(),
                transactionsRemindersid = this.transactionsRemindersid,
                done = this.done
            };
        }

        public static explicit operator ExpirationsReminders(ExpirationsRemindersDAO v)
        {
            return new ExpirationsReminders()
            {
                date = v.date,
                transactionsReminders = (v.transactionsReminders != null) ? (TransactionsReminders)v.transactionsReminders : null,
                transactionsRemindersid = v.transactionsRemindersid,
                done = v.done
            };
        }
    }
}
