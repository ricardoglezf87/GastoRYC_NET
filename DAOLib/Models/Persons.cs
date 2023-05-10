using System;
using System.ComponentModel.DataAnnotations;

namespace DAOLib.Models
{
    public class Persons
    {
        [Key]
        public virtual int id { set; get; }
        public virtual String? name { set; get; }
        public virtual int? categoryid { set; get; }
        public virtual Categories? category { set; get; }
    }
}
