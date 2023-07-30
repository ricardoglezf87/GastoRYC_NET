using System;

namespace GARCA.BO.Models
{
    public class VPortfolio : ModelBase
    {
        public virtual String? Description { set; get; }
        public virtual int? InvestmentProductsTypesid { set; get; }
        public virtual InvestmentProductsTypes? InvestmentProductsTypes { set; get; }
        public virtual String? Symbol { set; get; }
        public virtual DateTime? Date { set; get; }
        public virtual Decimal? Prices { set; get; }
        public virtual Decimal? NumShares { set; get; }
        public virtual Decimal? CostShares { set; get; }
        public virtual Decimal? MarketValue => (NumShares == null ? 0 : NumShares) * (Prices == null ? 0 : Prices);
        public virtual Decimal? Profit => (MarketValue == null ? 0 : MarketValue) - (CostShares == null ? 0 : CostShares);
        public virtual Decimal? ProfitPorcent => CostShares is null or 0 ? 0 : (MarketValue / CostShares - 1) * 100;
    }
}
