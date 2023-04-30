using BBDDLib.Models;
using BBDDLib.Models.Charts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using static GastosRYC.BBDDLib.Services.AccountsTypesService;

namespace GastosRYC.BBDDLib.Services
{
    public class ForecastsChartService
    {

        #region Propiedades y Contructor

        private readonly SimpleInjector.Container servicesContainer;

        public ForecastsChartService(SimpleInjector.Container servicesContainer)
        {
            this.servicesContainer = servicesContainer;
        }

        #endregion Propiedades y Contructor

        #region Functions

        public List<ForecastsChart> getMonthForecast()
        {
            Dictionary<Tuple<DateTime, int?>, Decimal> dChart = new();
            Dictionary<int, Decimal> saldos = new();

            DateTime now = DateTime.Now;

            foreach (var g in RYCContextService.getInstance().BBDD.accounts?.Where(x=> (x.closed == false || x.closed == null)
                                                                                   && ((x.accountsTypesid == (int)eAccountsTypes.Cash ||
                                                                                    x.accountsTypesid == (int)eAccountsTypes.Banks ||
                                                                                    x.accountsTypesid == (int)eAccountsTypes.Cards))))
            {
                saldos.Add(g.id, 0);
            }

            List<Transactions> remTransactions = new();

            foreach(ExpirationsReminders exp in servicesContainer.
                    GetInstance<ExpirationsRemindersService>().getAllPendingWithoutFutureWithGeneration())
            {
                
                remTransactions.AddRange(servicesContainer.
                    GetInstance<ExpirationsRemindersService>().
                    registerTransactionfromReminderSimulation(exp.id));
            }

            List<Transactions>? transactions = RYCContextService.getInstance().BBDD.transactions?.ToList();

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
                    servicesContainer.GetInstance<AccountsService>().getByID(key.Item2)?.description,
                    key.Item2, dChart[key]));
            }

            return lChart;
        }

        #endregion Functions

    }
}
