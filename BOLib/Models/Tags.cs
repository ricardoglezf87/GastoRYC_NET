using System;
using System.ComponentModel.DataAnnotations;

namespace BOLib.Models
{
    public class Tags
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? description { set; get; }

    }
}
