using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAOLib.Models
{
    [Table("Accounts")]
    public class AccountsDAO
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? description { set; get; }

        public virtual int? accountsTypesid { set; get; }

        public virtual AccountsTypesDAO? accountsTypes { set; get; }

        public virtual int? categoryid { set; get; }

        public virtual CategoriesDAO? category { set; get; }

        [DefaultValue(false)]
        public virtual Boolean? closed { set; get; }

        [NotMapped]
        public virtual Decimal balance { set; get; }
    }
}
