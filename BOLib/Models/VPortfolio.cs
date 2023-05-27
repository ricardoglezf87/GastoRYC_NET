using DAOLib.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOLib.Models
{
    public class VPortfolio : ModelBase
    {
        public virtual String? description { set; get; }
        public virtual String? symbol { set; get; }
        public virtual DateTime? date { set; get; }
        public virtual Decimal? prices { set; get; }
        public virtual Decimal? numShares { set; get; }
        public virtual Decimal? costShares { set; get; }
        public virtual Decimal? marketValue => (numShares == null ? 0 : numShares) * (prices == null ? 0 : prices);
        public virtual Decimal? profit => (marketValue == null ? 0 : marketValue) - (costShares == null ? 0 : costShares);
        public virtual Decimal? profitPorcent => (costShares == null || costShares == 0) ? 0 : ((marketValue / costShares) - 1) * 100;

        internal VPortfolioDAO toDAO()
        {
            return new VPortfolioDAO()
            {
                id = this.id,
                description = this.description,
                symbol = this.symbol,
                date = this.date,                
                prices = this.prices,
                numShares = this.numShares,
                costShares = this.costShares                
            };
        }

        public static explicit operator VPortfolio?(VPortfolioDAO? v)
        {
            return v == null
                ? null
                : new VPortfolio() {
                    id = v.id,
                    description = v.description,
                    symbol = v.symbol,
                    date = v.date,
                    prices = v.prices ?? 0,
                    numShares = v.numShares ?? 0,
                    costShares = (v.numShares == 0? 0 : v.costShares ?? 0)
                };
        }
    }
}
