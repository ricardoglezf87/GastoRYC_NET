using System;
using System.ComponentModel.DataAnnotations;

namespace BOLib.Models
{
    public class TransactionsStatus
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? description { set; get; }
    }
}
