
using BOLib.Models;
using DAOLib.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using static BOLib.Services.AccountsTypesService;

namespace BOLib.Services
{
    public class ForecastsChartService
    {

        #region Propiedades y Contructor

        private readonly ForecastsChartService forecastsChartService;
        private readonly TransactionsService transactionsService;
        private readonly ExpirationsRemindersService expirationsRemindersService;
        private readonly AccountsService accountService;

        public ForecastsChartService()
        {
            //TODO: Aqui creo que seria un forecast manager
            forecastsChartService = InstanceBase<ForecastsChartService>.Instance;
            transactionsService = InstanceBase<TransactionsService>.Instance;
            accountService = InstanceBase<AccountsService>.Instance;
            expirationsRemindersService = InstanceBase<ExpirationsRemindersService>.Instance;
        }

        #endregion Propiedades y Contructor

        #region Functions

        public List<ForecastsChart> getMonthForecast()
        {
            Dictionary<Tuple<DateTime, int?>, Decimal> dChart = new();
            Dictionary<int, Decimal> saldos = new();

            DateTime now = DateTime.Now;

            foreach (var g in accountService.getAll()?.Where(x => (x.closed == false || x.closed == null)
                                                            && ((x.accountsTypesid == (int)eAccountsTypes.Cash ||
                                                            x.accountsTypesid == (int)eAccountsTypes.Banks ||
                                                            x.accountsTypesid == (int)eAccountsTypes.Cards))))
            {
                saldos.Add(g.id, 0);
            }

            List<Transactions> remTransactions = new();

            foreach (ExpirationsReminders exp in expirationsRemindersService.getAllPendingWithoutFutureWithGeneration())
            {
                remTransactions.AddRange(expirationsRemindersService.registerTransactionfromReminderSimulation(exp));
            }

            List<Transactions>? transactions = transactionsService.getAll();

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
                                       && (x.account.closed == false || x.account.closed == null)
                                       && ((x.account.accountsTypesid == (int)eAccountsTypes.Cash ||
                                        x.account.accountsTypesid == (int)eAccountsTypes.Banks ||
                                        x.account.accountsTypesid == (int)eAccountsTypes.Cards)))
                                    .GroupBy(g => g.accountid))
                    {
                        Decimal saldo_act = (decimal)(g.Sum(x => x.amount) ?? 0);

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
                    accountService.getByID(key.Item2)?.description,
                    key.Item2, dChart[key]));
            }

            return lChart;
        }

        #endregion Functions

    }
}
