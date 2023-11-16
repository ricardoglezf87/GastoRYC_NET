namespace GARCA.Models
{
    public class VPortfolio : ModelBase<Int32>
    {
        public virtual string? Description { set; get; }
        public virtual int? InvestmentProductsTypesid { set; get; }
        public virtual InvestmentProductsTypes? InvestmentProductsTypes { set; get; }
        public virtual string? Symbol { set; get; }
        public virtual DateTime? Date { set; get; }
        public virtual decimal? Prices { set; get; }
        public virtual decimal? NumShares { set; get; }
        public virtual decimal? CostShares { set; get; }
        public virtual decimal? MarketValue => (NumShares ?? 0) * (Prices ?? 0);
        public virtual decimal? Profit => (MarketValue ?? 0) - (CostShares ?? 0);
        public virtual decimal? ProfitPorcent => CostShares is null or 0 ? 0 : (MarketValue / CostShares - 1) * 100;
    }
}
