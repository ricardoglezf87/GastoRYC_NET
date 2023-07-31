using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("Accounts")]
    public class AccountsDAO : ModelBaseDAO
    {
        [Column("description")]
        public virtual String? description { set; get; }

        [Column("accountsTypesid")]
        public virtual int? accountsTypesid { set; get; }

        [Column("accountsTypes")]
        public virtual AccountsTypesDAO? accountsTypes { set; get; }

        [Column("categoryid")]
        public virtual int? categoryid { set; get; }

        [Column("category")]
        public virtual CategoriesDAO? category { set; get; }

        [Column("closed")]
        [DefaultValue(false)]
        public virtual Boolean? closed { set; get; }
    }
}
