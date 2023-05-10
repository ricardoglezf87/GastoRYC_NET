using System;
using System.ComponentModel.DataAnnotations;

namespace BOLib.Models
{
    public class Persons
    {
        public virtual int id { set; get; }
        public virtual String? name { set; get; }
        public virtual int? categoryid { set; get; }
        public virtual Categories? category { set; get; }
    }
}
