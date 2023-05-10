using DAOLib.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace BOLib.Models
{
    public class TransactionsStatus
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? description { set; get; }

        public static explicit operator TransactionsStatus(TransactionsStatusDAO? v)
        {
            return new TransactionsStatus()
            {
                id = v.id,
                description = v.description
            };
        }
    }
}
