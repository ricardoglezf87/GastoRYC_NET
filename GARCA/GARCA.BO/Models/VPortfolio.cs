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
        public virtual Decimal? MarketValue => (NumShares ?? 0) * (Prices ?? 0);
        public virtual Decimal? Profit => (MarketValue ?? 0) - (CostShares ?? 0);
        public virtual Decimal? ProfitPorcent => CostShares is null or 0 ? 0 : (MarketValue / CostShares - 1) * 100;
    }
}
