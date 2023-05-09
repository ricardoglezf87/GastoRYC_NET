using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BBDDLib.Models
{
    public class Accounts
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? description { set; get; }

        public virtual int? accountsTypesid { set; get; }

        public virtual AccountsTypes? accountsTypes { set; get; }

        public virtual int? categoryid { set; get; }

        public virtual Categories? category { set; get; }

        [DefaultValue(false)]
        public virtual Boolean? closed { set; get; }

        [NotMapped]
        public virtual Decimal balance { set; get; }
    }
}
