using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class TransactionsStatus : ModelBase
    {
        public virtual String? description { set; get; }
        
        public static explicit operator TransactionsStatus?(TransactionsStatusDAO? v)
        {
            return v == null
                ? null
                : new TransactionsStatus
                {
                    id = v.id,
                    description = v.description
                };
        }
    }
}
