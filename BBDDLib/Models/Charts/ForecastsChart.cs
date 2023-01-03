using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBDDLib.Models.Charts
{
    public class ForecastsChart
    {
        public virtual DateTime? date { set; get; }
        public virtual int? accountid { set; get; }
        public virtual String? account { set; get; }
        public virtual Decimal? amount { set; get; }

        public ForecastsChart(DateTime? date,String? account, decimal? amount)
        {
            this.date = date;
            this.account = account;
            this.amount = amount;
        }

        public ForecastsChart(DateTime? date, String? account, int? accountid, decimal? amount)
        {
            this.date = date;
            this.account = account;
            this.amount = amount;
            this.accountid = accountid;
        }
    }
}
