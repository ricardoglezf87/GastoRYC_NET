using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("AccountsTypes")]
    public class AccountsTypesDao : ModelBaseDao
    {
        [Column("description")]
        public virtual String? Description { set; get; }
    }
}
