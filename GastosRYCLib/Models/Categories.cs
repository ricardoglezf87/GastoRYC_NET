using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYCLib.Models
{
    public class Categories : ICollection
    {
        [Key]
        public virtual long id { set; get; }

        public virtual String? description { set; get; }

    }
}
