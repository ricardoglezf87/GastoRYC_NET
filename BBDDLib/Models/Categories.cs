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
    public class Categories
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? description { set; get; }

        public virtual int? categoriesTypesid { set; get; }

        public virtual CategoriesTypes? categoriesTypes { set; get; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Accounts? accounts { set; get; }

    }
}
