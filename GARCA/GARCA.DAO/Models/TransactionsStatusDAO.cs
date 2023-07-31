using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("TransactionsStatus")]
    public class TransactionsStatusDAO : ModelBaseDAO
    {
        [Column("description")]
        public virtual String? description { set; get; }
    }
}
