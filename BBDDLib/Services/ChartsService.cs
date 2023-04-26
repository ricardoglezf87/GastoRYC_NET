using BBDDLib.Models;
using BBDDLib.Models.Charts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Linq;
using static GastosRYC.BBDDLib.Services.AccountsTypesService;

namespace GastosRYC.BBDDLib.Services
{
    public class ChartsService
    {

        #region Propiedades y Contructor

        private readonly SimpleInjector.Container servicesContainer;

        public ChartsService(SimpleInjector.Container servicesContainer)
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

            foreach (var g in RYCContextService.getInstance().BBDD.accounts)
            {
                saldos.Add(g.id, 0);
            }

            for (int i = 0; i < 30; i++)
            {
                DateTime d = now.AddDays(i);
                foreach (var g in RYCContextService.getInstance().BBDD.transactions?
                                .Where(x => x.category != null && x.date <= d
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

            //for (int i = 1; i < 30; i++)
            //{
            //    DateTime d = now.AddDays(i);
            //    foreach (var g in RYCContextService.getInstance().BBDD.transactions?
            //                    .Where(x => x.category != null && x.date <= d
            //                       && ((x.account.accountsTypesid == (int)eAccountsTypes.Cash ||
            //                        x.account.accountsTypesid == (int)eAccountsTypes.Banks ||
            //                        x.account.accountsTypesid == (int)eAccountsTypes.Cards)))
            //                    .GroupBy(g => g.accountid))
            //    {
            //        dChart[new Tuple<DateTime, int?>(d, g.Key)] += dChart[new Tuple<DateTime, int?>(d.AddDays(-1), g.Key)]
            //                + (decimal)(g.Sum(x => x.amount) ?? 0);
            //    }
            //}

            //foreach (var g in RYCContextService.getInstance().BBDD.transactions?
            //                    .Where(x => x.category != null && x.date <= DateTime.Today
            //                       && ((x.account.accountsTypesid == (int)eAccountsTypes.Cash ||
            //                        x.account.accountsTypesid == (int)eAccountsTypes.Banks ||
            //                        x.account.accountsTypesid == (int)eAccountsTypes.Cards)))
            //                    .GroupBy(g => g.accountid))
            //{
            //    //lChart.Add(new ForecastsChart(DateTime.Today.AddDays(-1), servicesContainer.GetInstance<AccountsService>().getByID(g.Key)?.description,
            //    //    g.Key, g.Sum(x => x.amount)));
            //    lChart.Add(new ForecastsChart(DateTime.Today, servicesContainer.GetInstance<AccountsService>().getByID(g.Key)?.description,
            //        g.Key, g.Sum(x => x.amount)));
            //}           

            //foreach (var g in RYCContextService.getInstance().BBDD.transactions?
            //                    .Where(x => x.category != null && x.date > DateTime.Today
            //                       && ((x.account.accountsTypesid == (int)eAccountsTypes.Cash ||
            //                        x.account.accountsTypesid == (int)eAccountsTypes.Banks ||
            //                        x.account.accountsTypesid == (int)eAccountsTypes.Cards)))
            //                    .GroupBy(g => new { g.date, g.accountid }))
            //{
            //    lChart.Add(new ForecastsChart(g.Key.date, servicesContainer.GetInstance<AccountsService>().getByID(g.Key.accountid)?.description,
            //        g.Key.accountid, g.Sum(x => x.amount)));
            //}

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
