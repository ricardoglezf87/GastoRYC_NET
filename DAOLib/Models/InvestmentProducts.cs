using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DAOLib.Models
{
    public class InvestmentProducts
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? description { set; get; }

        public virtual String? symbol { set; get; }

        public virtual String? url { set; get; }
        
        [DefaultValue(true)]
        public virtual bool? active { set; get; }
    }
}
