using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("AccountsTypes")]
    public class AccountsTypesDAO : ModelBaseDAO
    {
        public virtual String? description { set; get; }
    }
}
