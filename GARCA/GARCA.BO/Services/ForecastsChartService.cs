
using GARCA.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static GARCA.BO.Services.AccountsTypesService;
using GARCA.Utils.IOC;

namespace GARCA.BO.Services
{
    public class ForecastsChartService
    {
        #region Functions

        public List<ForecastsChart> getMonthForecast()
        {
            Dictionary<Tuple<DateTime, int?>, Decimal> dChart = new();
            Dictionary<int, Decimal> saldos = new();

            DateTime now = DateTime.Now;

            foreach (var g in DependencyConfig.iAccountsService.getAllOpened()?.Where(x => (x.closed == false || x.closed == null)
                                                            && (x.accountsTypesid == (int)eAccountsTypes.Cash ||
                                                            x.accountsTypesid == (int)eAccountsTypes.Banks ||
                                                            x.accountsTypesid == (int)eAccountsTypes.Cards)))
            {
                saldos.Add(g.id, 0);
            }

            List<Transactions> remTransactions = new();

            foreach (ExpirationsReminders? exp in DependencyConfig.iExpirationsRemindersService.getAllPendingWithoutFutureWithGeneration())
            {
                if (exp != null)
                {
                    remTransactions.AddRange(DependencyConfig.iExpirationsRemindersService.registerTransactionfromReminderSimulation(exp));
                }
            }

            List<Transactions?>? transactions = DependencyConfig.iTransactionsService.getAll()?.ToList();

            if (transactions != null)
            {
                if (remTransactions != null && remTransactions.Count > 0)
                {
                    transactions.AddRange(remTransactions);
                }

                for (int i = 0; i < 30; i++)
                {
                    DateTime d = now.AddDays(i);
                    foreach (var g in transactions
                                    .Where(x => x.category != null && x.date <= d
                                       && (x.account?.closed == false || x.account?.closed == null)
                                       && (x.account?.accountsTypesid == (int)eAccountsTypes.Cash ||
                                        x.account?.accountsTypesid == (int)eAccountsTypes.Banks ||
                                        x.account?.accountsTypesid == (int)eAccountsTypes.Cards))
                                    .GroupBy(g => g.accountid))
                    {
                        Decimal saldo_act = g.Sum(x => x.amount) ?? 0;

                        if (g.Key != null && (saldos[g.Key.Value] != saldo_act || i == 29))
                        {
                            dChart.Add(new Tuple<DateTime, int?>(d, g.Key), saldo_act);
                            saldos[g.Key.Value] = saldo_act;
                        }
                    }
                }
            }

            List<ForecastsChart> lChart = new();

            foreach (Tuple<DateTime, int?> key in dChart.Keys)
            {
                lChart.Add(new ForecastsChart(key.Item1,
                    DependencyConfig.iAccountsService.getByID(key.Item2)?.description,
                    key.Item2, dChart[key]));
            }
            return lChart;
        }

        #endregion Functions

    }
}
