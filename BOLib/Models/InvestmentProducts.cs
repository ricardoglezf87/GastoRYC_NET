using DAOLib.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BOLib.Models
{
    public class InvestmentProducts : ModelBase
    {
        public virtual String? description { set; get; }

        public virtual String? symbol { set; get; }

        public virtual String? url { set; get; }
        
        [DefaultValue(true)]
        public virtual bool? active { set; get; }

        internal InvestmentProductsDAO toDAO()
        {
            return new InvestmentProductsDAO()
            {
                id = this.id,
                description = this.description,
                symbol = this.symbol,
                url = this.url
            };
        }

        public static explicit operator InvestmentProducts(InvestmentProductsDAO? v)
        {
            return new InvestmentProducts()
            {
                id = v.id,
                description = v.description,
                symbol = v.symbol,
                url = v.url
            };
        }
    }
}
