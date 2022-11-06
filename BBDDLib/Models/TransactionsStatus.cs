using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBDDLib.Models
{
    public class TransactionsStatus
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? description { set; get; }
    }
}
