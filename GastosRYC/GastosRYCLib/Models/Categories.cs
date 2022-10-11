using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GastosRYCLib.Models
{
    public class Categories
    {
        [Key]
        public virtual long id { set; get; }

        public virtual String? description { set; get; }

    }
}
