using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BBDDLib.Models
{
    public class CategoriesTypes
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? description { set; get; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual List<Categories>? categories { set; get; }
    }
}
