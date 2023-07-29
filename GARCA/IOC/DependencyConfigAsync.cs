using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GARCA.BO.Services;

namespace GARCA.IOC
{
    public static class DependencyConfigAsync
    {
        private static readonly Container container;

        public static AccountsService iAccountsService
        {
            get { return container.GetInstance<AccountsService>(); }
        }

        public static AccountsTypesService iAccountsTypesService
        {
            get { return container.GetInstance<AccountsTypesService>(); }
        }

        public static CategoriesService iCategoriesService
        {
            get { return container.GetInstance<CategoriesService>(); }
        }

        public static CategoriesTypesService iCategoriesTypesService
        {
            get { return container.GetInstance<CategoriesTypesService>(); }
        }

        public static DateCalendarService iDateCalendarService
        {
            get { return container.GetInstance<DateCalendarService>(); }
        }

        public static ExpirationsRemindersService iExpirationsRemindersService
        {
            get { return container.GetInstance<ExpirationsRemindersService>(); }
        }

        public static ForecastsChartService iForecastsChartService
        {
            get { return container.GetInstance<ForecastsChartService>(); }
        }

        public static InvestmentProductsPricesService iInvestmentProductsPricesService
        {
            get { return container.GetInstance<InvestmentProductsPricesService>(); }
        }

        public static InvestmentProductsService iInvestmentProductsService
        {
            get { return container.GetInstance<InvestmentProductsService>(); }
        }

        public static InvestmentProductsTypesService iInvestmentProductsTypesService
        {
            get { return container.GetInstance<InvestmentProductsTypesService>(); }
        }

        public static PeriodsRemindersService iPeriodsReminderService
        {
            get { return container.GetInstance<PeriodsRemindersService>(); }
        }

        public static PersonsService iPersonsService
        {
            get { return container.GetInstance<PersonsService>(); }
        }

        public static RYCContextService iRYCContextService
        {
            get { return container.GetInstance<RYCContextService>(); }
        }

        public static SplitsRemindersService iSplitsRemindersService
        {
            get { return container.GetInstance<SplitsRemindersService>(); }
        }

        public static SplitsService iSplitsService
        {
            get { return container.GetInstance<SplitsService>(); }
        }

        public static TagsService iTagsService
        {
            get { return container.GetInstance<TagsService>(); }
        }
        public static TransactionsRemindersService iTransactionsRemindersService
        {
            get { return container.GetInstance<TransactionsRemindersService>(); }
        }
        public static TransactionsService iTransactionsService
        {
            get { return container.GetInstance<TransactionsService>(); }
        }
        public static TransactionsStatusService iTransactionsStatusService
        {
            get { return container.GetInstance<TransactionsStatusService>(); }
        }
        public static VBalancebyCategoryService iVBalancebyCategoryService
        {
            get { return container.GetInstance<VBalancebyCategoryService>(); }
        }
        public static VPortfolioService iVPortfolioService
        {
            get { return container.GetInstance<VPortfolioService>(); }
        }

        static DependencyConfigAsync()
        {
            container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            RegisterDependencies();
        }

        private static void RegisterDependencies()
        {
            container.Register<AccountsService>(Lifestyle.Scoped);
            container.Register<AccountsTypesService>(Lifestyle.Scoped);
            container.Register<CategoriesService>(Lifestyle.Scoped);
            container.Register<CategoriesTypesService>(Lifestyle.Scoped);
            container.Register<DateCalendarService>(Lifestyle.Scoped);
            container.Register<ExpirationsRemindersService>(Lifestyle.Scoped);
            container.Register<ForecastsChartService>(Lifestyle.Scoped);
            container.Register<InvestmentProductsPricesService>(Lifestyle.Scoped);
            container.Register<InvestmentProductsService>(Lifestyle.Scoped);
            container.Register<InvestmentProductsTypesService>(Lifestyle.Scoped);
            container.Register<PeriodsRemindersService>(Lifestyle.Scoped);
            container.Register<PersonsService>(Lifestyle.Scoped);
            container.Register<RYCContextService>(Lifestyle.Scoped);
            container.Register<SplitsRemindersService>(Lifestyle.Scoped);
            container.Register<SplitsService>(Lifestyle.Scoped);
            container.Register<TagsService>(Lifestyle.Scoped);
            container.Register<TransactionsRemindersService>(Lifestyle.Scoped);
            container.Register<TransactionsService>(Lifestyle.Scoped);
            container.Register<TransactionsStatusService>(Lifestyle.Scoped);
            container.Register<VBalancebyCategoryService>(Lifestyle.Scoped);
            container.Register<VPortfolioService>(Lifestyle.Scoped);
        }


    }
}
