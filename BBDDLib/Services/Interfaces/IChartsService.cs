using BBDDLib.Models;
using BBDDLib.Models.Charts;
using System.Collections.Generic;

namespace GastosRYC.BBDDLib.Services
{
    public interface IChartsService
    {
        public List<ExpensesChart> getExpenses();

        public List<ForecastsChart> getMonthForecast();

    }
}
