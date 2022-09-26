using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYCLib.Models
{
    public class Accounts : ICollection
    {
        [Key]
        public virtual long id { set; get; }

        public virtual String? description { set; get; }

        public virtual AccountsTypes? accountsTypes { set; get; }

        public virtual  Decimal balance { set; get; }

    }
}
