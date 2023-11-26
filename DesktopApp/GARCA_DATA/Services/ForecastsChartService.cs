using GARCA.Models;
using static GARCA.Data.IOC.DependencyConfig;
using static GARCA.Data.Services.AccountsTypesService;

namespace GARCA.Data.Services
{
    public class ForecastsChartService
    {
        #region Functions

        public async Task<IEnumerable<ForecastsChart>> GetMonthForecast()
        {
            Dictionary<Tuple<DateTime, int?>, Decimal> dChart = new();
            Dictionary<int, Decimal> saldos = new();

            var now = DateTime.Now;

            foreach (var g in (await iAccountsService.GetAllOpened() ?? Enumerable.Empty<Accounts>())?.Where(x =>
                                                            x.AccountsTypesId is ((int)EAccountsTypes.Cash) or
                                                            ((int)EAccountsTypes.Banks) or
                                                            ((int)EAccountsTypes.Cards)))
            {
                saldos.Add(g.Id, 0);
            }

            List<Transactions> remTransactions = new();

            foreach (var exp in await iExpirationsRemindersService.GetAllPendingWithoutFutureWithGeneration())
            {
                if (exp != null)
                {
                    remTransactions.AddRange(await iExpirationsRemindersService.RegisterTransactionfromReminderSimulation(exp));
                }
            }

            var transactions = (await iTransactionsService.GetAll())?.ToList();

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
                                    .Where(x => x.Categories != null && x.Date <= d
                                       && (x.Accounts?.Closed == false || x.Accounts?.Closed == null)
                                       && (x.Accounts?.AccountsTypesId == (int)EAccountsTypes.Cash ||
                                        x.Accounts?.AccountsTypesId == (int)EAccountsTypes.Banks ||
                                        x.Accounts?.AccountsTypesId == (int)EAccountsTypes.Cards))
                                    .GroupBy(g => g.AccountsId))
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
                    (await iAccountsService.GetById(key.Item2 ?? -99))?.Description,
                    key.Item2, dChart[key]));
            }
            return lChart;
        }

        #endregion Functions

    }
}
