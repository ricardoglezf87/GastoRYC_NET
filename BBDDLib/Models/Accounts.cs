using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BBDDLib.Models
{
    public class Accounts
    {
        [Key]
        public virtual long id { set; get; }

        public virtual String? description { set; get; }

        public virtual long accountsTypesid { set; get; }

        public virtual AccountsTypes? accountsTypes { set; get; }

        [NotMapped]
        public virtual  Decimal balance { set; get; }
    }
}
