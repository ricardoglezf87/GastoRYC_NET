using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAOLib.Models
{
    [Table("AccountsTypes")]
    public class AccountsTypesDAO : ModelBaseDAO
    {
        public virtual String? description { set; get; }
    }
}
