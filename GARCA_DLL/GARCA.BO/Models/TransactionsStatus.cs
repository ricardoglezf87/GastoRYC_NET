using GARCA.DAO.Models;

namespace GARCA.BO.Models
{
    public class TransactionsStatus : ModelBase
    {
        public virtual String? Description { set; get; }

        public static explicit operator TransactionsStatus?(TransactionsStatusDao? v)
        {
            return v == null
                ? null
                : new TransactionsStatus
                {
                    Id = v.Id,
                    Description = v.Description
                };
        }
    }
}
