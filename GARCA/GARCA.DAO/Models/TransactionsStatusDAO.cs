using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("TransactionsStatus")]
    public class TransactionsStatusDao : ModelBaseDao
    {
        [Column("description")]
        public virtual String? Description { set; get; }
    }
}
