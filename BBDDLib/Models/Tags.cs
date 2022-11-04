using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BBDDLib.Models
{
    public class Tags
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? description { set; get; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Transactions? transactions { set; get; }

    }
}
