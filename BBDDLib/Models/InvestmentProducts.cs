using System;
using System.ComponentModel.DataAnnotations;

namespace BBDDLib.Models
{
    public class InvestmentProducts
    {
        [Key]
        public virtual int id { set; get; }
        public virtual String? description { set; get; }

    }
}
