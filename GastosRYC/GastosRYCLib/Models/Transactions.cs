using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYCLib.Models
{
    public class Transactions : ICollection
    {
        public virtual long id { set; get; }

        public virtual DateTime date { set; get; }

        public virtual Accounts? account { set; get; }

        public virtual Persons? person { set; get; }

        public virtual Categories? category { set; get; }

        public virtual Decimal? amount { set; get; }

        public virtual Double? orden { set; get; }

        [NotMapped]
        public virtual Decimal? balance { set; get; }

    }
}
