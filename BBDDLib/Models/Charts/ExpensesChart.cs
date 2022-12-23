using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBDDLib.Models.Charts
{
    public class ExpensesChart
    {
        public virtual int? año { set; get; }
        public virtual int? mes { set; get; }
        public virtual int? categoryid { set; get; }
        public virtual String? category { set; get; }
        public virtual Decimal? amount { set; get; }

        public ExpensesChart(int? año, int? mes, int? categoryid, String? category, decimal? amount)
        {
            this.año = año;
            this.mes = mes;
            this.categoryid = categoryid;
            this.category = category;
            this.amount = amount;
        }

        public ExpensesChart(String? category, decimal? amount)
        {
            this.category = category;
            this.amount = amount;
        }
    }
}
