using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class TransactionsStatus : ModelBase
    {
        public virtual String? Description { set; get; }
        
        public static explicit operator TransactionsStatus?(TransactionsStatusDAO? v)
        {
            return v == null
                ? null
                : new TransactionsStatus
                {
                    Id = v.id,
                    Description = v.description
                };
        }
    }
}
