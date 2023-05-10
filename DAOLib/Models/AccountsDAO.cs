using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAOLib.Models
{
    [Table("Accounts")]
    public class AccountsDAO : IModelDAO
    {
        public virtual String? description { set; get; }

        public virtual int? accountsTypesid { set; get; }

        public virtual AccountsTypesDAO? accountsTypes { set; get; }

        public virtual int? categoryid { set; get; }

        public virtual CategoriesDAO? category { set; get; }

        [DefaultValue(false)]
        public virtual Boolean? closed { set; get; }
    }
}
