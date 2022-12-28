using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBDDLib.Models
{
    public class ExpirationsReminders
    {
        [Key]
        public virtual int id { set; get; }

        public virtual DateTime? date { set; get; }

        public virtual int? transactionsRemindersid { set; get; }

        public virtual TransactionsReminders? transactionsReminders { set; get; }

        public virtual bool? done { set; get; }

        [NotMapped]
        public virtual String? groupDate 
        { get 
            {
                String group = String.Empty;

                if (date != null)
                {
                    if (date < DateTime.Now)
                    {
                        return "Vencido";
                    }
                    else if (date > DateTime.Now.AddMonths(1))
                    {
                        return "Futuro";
                    }
                    else
                    {
                        return date.Value.Day + "/" + date.Value.Month;
                    }
                }

                return group;
            } 
        }

    }
}
