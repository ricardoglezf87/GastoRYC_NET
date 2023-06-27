using System;

namespace GARCA.BO.Models
{
    public class ForecastsChart
    {
        public virtual DateTime? date { set; get; }
        public virtual int? accountid { set; get; }
        public virtual string? account { set; get; }
        public virtual decimal? amount { set; get; }

        public ForecastsChart(DateTime? date, string? account, decimal? amount)
        {
            this.date = date;
            this.account = account;
            this.amount = amount;
        }

        public ForecastsChart(DateTime? date, string? account, int? accountid, decimal? amount)
        {
            this.date = date;
            this.account = account;
            this.amount = amount;
            this.accountid = accountid;
        }
    }
}
