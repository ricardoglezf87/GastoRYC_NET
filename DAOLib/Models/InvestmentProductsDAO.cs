using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAOLib.Models
{
    [Table("InvestmentProducts")]
    public class InvestmentProductsDAO : IModelDAO
    {
        public virtual String? description { set; get; }

        public virtual String? symbol { set; get; }

        public virtual String? url { set; get; }
        
        [DefaultValue(true)]
        public virtual bool? active { set; get; }
    }
}
