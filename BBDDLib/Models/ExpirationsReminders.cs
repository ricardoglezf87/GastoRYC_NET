using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public virtual int? transactaionsRemindersid { set; get; }

        public virtual TransactionsReminders? transactaionsReminders { set; get; }

        public virtual bool? done { set; get; }

    }
}
