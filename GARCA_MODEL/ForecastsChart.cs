namespace GARCA.Models
{
    public class ForecastsChart
    {
        public virtual DateTime? Date { set; get; }
        public virtual int? Accountid { set; get; }
        public virtual string? Account { set; get; }
        public virtual decimal? Amount { set; get; }

        public ForecastsChart(DateTime? date, string? account, int? accountid, decimal? amount)
        {
            Date = date;
            Account = account;
            Amount = amount;
            Accountid = accountid;
        }
    }
}
