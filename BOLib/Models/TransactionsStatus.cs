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

        internal TransactionsStatusDAO toDAO() 
        {
            return new TransactionsStatusDAO()
            {
                id = this.id,
                description = this.description
            };
        }

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
