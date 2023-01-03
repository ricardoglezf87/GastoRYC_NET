using BBDDLib.Models;
using BBDDLib.Models.Charts;
using BBDDLib.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using static GastosRYC.BBDDLib.Services.IAccountsTypesService;

namespace GastosRYC.BBDDLib.Services
{
    public class ChartsService : IChartsService
    {

        #region Propiedades y Contructor

        private readonly SimpleInjector.Container servicesContainer;

        public ChartsService(SimpleInjector.Container servicesContainer)
        {
            this.servicesContainer = servicesContainer;
        }

        #endregion Propiedades y Contructor

        #region Functions

        public List<ExpensesChart> getExpenses()
        {
            List<ExpensesChart> lChart = new();

            foreach (var g in RYCContextService.getInstance().BBDD.transactions?
                                .Where(x => x.category != null && x.category.categoriesTypesid == (int)ICategoriesTypesService.eCategoriesTypes.Expenses)
                                .GroupBy(g => g.category))
            {
                lChart.Add(new ExpensesChart(g.Key.description, -g.Sum(x => x.amount)));
            }

            return lChart;
        }

        public List<ForecastsChart> getMonthForecast()
        {
            List<ForecastsChart> lChart = new();

            foreach (var g in RYCContextService.getInstance().BBDD.transactions?
                                .Where(x => x.category != null && x.date <= DateTime.Today
                                   && ((x.account.accountsTypesid == (int)eAccountsTypes.Cash ||
                                    x.account.accountsTypesid == (int)eAccountsTypes.Banks ||
                                    x.account.accountsTypesid == (int)eAccountsTypes.Cards)))
                                .GroupBy(g => g.accountid))
            {
                //lChart.Add(new ForecastsChart(DateTime.Today.AddDays(-1), servicesContainer.GetInstance<IAccountsService>().getByID(g.Key)?.description,
                //    g.Key, g.Sum(x => x.amount)));
                lChart.Add(new ForecastsChart(DateTime.Today, servicesContainer.GetInstance<IAccountsService>().getByID(g.Key)?.description,
                    g.Key, g.Sum(x => x.amount)));
            }           

            foreach (var g in RYCContextService.getInstance().BBDD.transactions?
                                .Where(x => x.category != null && x.date > DateTime.Today
                                   && ((x.account.accountsTypesid == (int)eAccountsTypes.Cash ||
                                    x.account.accountsTypesid == (int)eAccountsTypes.Banks ||
                                    x.account.accountsTypesid == (int)eAccountsTypes.Cards)))
                                .GroupBy(g => new { g.date, g.accountid }))
            {
                lChart.Add(new ForecastsChart(g.Key.date, servicesContainer.GetInstance<IAccountsService>().getByID(g.Key.accountid)?.description,
                    g.Key.accountid, g.Sum(x => x.amount)));
            }

            return lChart;
        }        

        #endregion Functions

    }
}
