using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;
using static GARCA.Data.Services.AccountsTypesService;

namespace GARCA.Data.Services
{
    public class ForecastsChartService
    {
        #region Functions

        public HashSet<ForecastsChart> GetMonthForecast()
        {
            Dictionary<Tuple<DateTime, int?>, Decimal> dChart = new();
            Dictionary<int, Decimal> saldos = new();

            var now = DateTime.Now;

            foreach (var g in (iAccountsService.GetAllOpened() ?? Enumerable.Empty<Accounts>())?.Where(x => 
                                                            x.AccountsTypesid == (int)EAccountsTypes.Cash ||
                                                            x.AccountsTypesid == (int)EAccountsTypes.Banks ||
                                                            x.AccountsTypesid == (int)EAccountsTypes.Cards))
            {
                saldos.Add(g.Id, 0);
            }

            List<Transactions> remTransactions = new();

            foreach (var exp in iExpirationsRemindersService.GetAllPendingWithoutFutureWithGeneration())
            {
                if (exp != null)
                {
                    remTransactions.AddRange(iExpirationsRemindersService.RegisterTransactionfromReminderSimulation(exp));
                }
            }

            var transactions = iTransactionsService.GetAll()?.ToList();

            if (transactions != null)
            {
                if (remTransactions.Count > 0)
                {
                    transactions.AddRange(remTransactions);
                }

                for (var i = 0; i < 30; i++)
                {
                    var d = now.AddDays(i);
                    foreach (var g in transactions
                                    .Where(x => x.Category != null && x.Date <= d
                                       && (x.Account?.Closed == false || x.Account?.Closed == null)
                                       && (x.Account?.AccountsTypesid == (int)EAccountsTypes.Cash ||
                                        x.Account?.AccountsTypesid == (int)EAccountsTypes.Banks ||
                                        x.Account?.AccountsTypesid == (int)EAccountsTypes.Cards))
                                    .GroupBy(g => g.Accountid))
                    {
                        var saldoAct = g.Sum(x => x.Amount) ?? 0;

                        if (g.Key != null && (saldos[g.Key.Value] != saldoAct || i == 29))
                        {
                            dChart.Add(new Tuple<DateTime, int?>(d, g.Key), saldoAct);
                            saldos[g.Key.Value] = saldoAct;
                        }
                    }
                }
            }

            HashSet<ForecastsChart> lChart = new();

            foreach (var key in dChart.Keys)
            {
                lChart.Add(new ForecastsChart(key.Item1,
                    iAccountsService.GetById(key.Item2 ?? -99)?.Description,
                    key.Item2, dChart[key]));
            }
            return lChart;
        }

        #endregion Functions

    }
}
