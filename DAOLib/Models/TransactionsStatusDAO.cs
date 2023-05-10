using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAOLib.Models
{
    [Table("TransactionsStatus")]
    public class TransactionsStatusDAO
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? description { set; get; }
    }
}
