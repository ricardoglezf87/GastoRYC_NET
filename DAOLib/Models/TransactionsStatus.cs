using System;
using System.ComponentModel.DataAnnotations;

namespace DAOLib.Models
{
    public class TransactionsStatus
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? description { set; get; }
    }
}
