using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GARCA.BO.Services;

namespace GARCA.Utils.IOC
{
    public static class DependencyConfig
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

        static DependencyConfig()
        {
            container = new Container();
            RegisterDependencies();
        }

        private static void RegisterDependencies()
        {
            container.Register<AccountsService>(Lifestyle.Singleton);
            container.Register<AccountsTypesService>(Lifestyle.Singleton);
            container.Register<CategoriesService>(Lifestyle.Singleton);
            container.Register<CategoriesTypesService>(Lifestyle.Singleton);
            container.Register<DateCalendarService>(Lifestyle.Singleton);
            container.Register<ExpirationsRemindersService>(Lifestyle.Singleton);
            container.Register<ForecastsChartService>(Lifestyle.Singleton);
            container.Register<InvestmentProductsPricesService>(Lifestyle.Singleton);
            container.Register<InvestmentProductsService>(Lifestyle.Singleton);
            container.Register<InvestmentProductsTypesService>(Lifestyle.Singleton);
            container.Register<PeriodsRemindersService>(Lifestyle.Singleton);
            container.Register<PersonsService>(Lifestyle.Singleton);
            container.Register<RYCContextService>(Lifestyle.Singleton);
            container.Register<SplitsRemindersService>(Lifestyle.Singleton);
            container.Register<SplitsService>(Lifestyle.Singleton);
            container.Register<TagsService>(Lifestyle.Singleton);
            container.Register<TransactionsRemindersService>(Lifestyle.Singleton);
            container.Register<TransactionsService>(Lifestyle.Singleton);
            container.Register<TransactionsStatusService>(Lifestyle.Singleton);
            container.Register<VBalancebyCategoryService>(Lifestyle.Singleton);
            container.Register<VPortfolioService>(Lifestyle.Singleton);
        }


    }
}
