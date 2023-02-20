using System;
using System.ComponentModel.DataAnnotations;

namespace BBDDLib.Models
{
    public class Persons
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? name { set; get; }
    }
}
