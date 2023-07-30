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
        private static readonly Container Container;

        public static AccountsService IAccountsService
        {
            get { return Container.GetInstance<AccountsService>(); }
        }

        public static AccountsTypesService IAccountsTypesService
        {
            get { return Container.GetInstance<AccountsTypesService>(); }
        }

        public static CategoriesService ICategoriesService
        {
            get { return Container.GetInstance<CategoriesService>(); }
        }

        public static CategoriesTypesService ICategoriesTypesService
        {
            get { return Container.GetInstance<CategoriesTypesService>(); }
        }

        public static DateCalendarService IDateCalendarService
        {
            get { return Container.GetInstance<DateCalendarService>(); }
        }

        public static ExpirationsRemindersService IExpirationsRemindersService
        {
            get { return Container.GetInstance<ExpirationsRemindersService>(); }
        }

        public static ForecastsChartService IForecastsChartService
        {
            get { return Container.GetInstance<ForecastsChartService>(); }
        }

        public static InvestmentProductsPricesService IInvestmentProductsPricesService
        {
            get { return Container.GetInstance<InvestmentProductsPricesService>(); }
        }

        public static InvestmentProductsService IInvestmentProductsService
        {
            get { return Container.GetInstance<InvestmentProductsService>(); }
        }

        public static InvestmentProductsTypesService IInvestmentProductsTypesService
        {
            get { return Container.GetInstance<InvestmentProductsTypesService>(); }
        }

        public static PeriodsRemindersService IPeriodsReminderService
        {
            get { return Container.GetInstance<PeriodsRemindersService>(); }
        }

        public static PersonsService IPersonsService
        {
            get { return Container.GetInstance<PersonsService>(); }
        }

        public static RycContextService IRycContextService
        {
            get { return Container.GetInstance<RycContextService>(); }
        }

        public static SplitsRemindersService ISplitsRemindersService
        {
            get { return Container.GetInstance<SplitsRemindersService>(); }
        }

        public static SplitsService ISplitsService
        {
            get { return Container.GetInstance<SplitsService>(); }
        }

        public static TagsService ITagsService
        {
            get { return Container.GetInstance<TagsService>(); }
        }
        public static TransactionsRemindersService ITransactionsRemindersService
        {
            get { return Container.GetInstance<TransactionsRemindersService>(); }
        }
        public static TransactionsService ITransactionsService
        {
            get { return Container.GetInstance<TransactionsService>(); }
        }
        public static TransactionsStatusService ITransactionsStatusService
        {
            get { return Container.GetInstance<TransactionsStatusService>(); }
        }
        public static VBalancebyCategoryService IVBalancebyCategoryService
        {
            get { return Container.GetInstance<VBalancebyCategoryService>(); }
        }
        public static VPortfolioService IVPortfolioService
        {
            get { return Container.GetInstance<VPortfolioService>(); }
        }

        static DependencyConfig()
        {
            Container = new Container();
            RegisterDependencies();
        }

        private static void RegisterDependencies()
        {
            Container.Register<AccountsService>(Lifestyle.Singleton);
            Container.Register<AccountsTypesService>(Lifestyle.Singleton);
            Container.Register<CategoriesService>(Lifestyle.Singleton);
            Container.Register<CategoriesTypesService>(Lifestyle.Singleton);
            Container.Register<DateCalendarService>(Lifestyle.Singleton);
            Container.Register<ExpirationsRemindersService>(Lifestyle.Singleton);
            Container.Register<ForecastsChartService>(Lifestyle.Singleton);
            Container.Register<InvestmentProductsPricesService>(Lifestyle.Singleton);
            Container.Register<InvestmentProductsService>(Lifestyle.Singleton);
            Container.Register<InvestmentProductsTypesService>(Lifestyle.Singleton);
            Container.Register<PeriodsRemindersService>(Lifestyle.Singleton);
            Container.Register<PersonsService>(Lifestyle.Singleton);
            Container.Register<RycContextService>(Lifestyle.Singleton);
            Container.Register<SplitsRemindersService>(Lifestyle.Singleton);
            Container.Register<SplitsService>(Lifestyle.Singleton);
            Container.Register<TagsService>(Lifestyle.Singleton);
            Container.Register<TransactionsRemindersService>(Lifestyle.Singleton);
            Container.Register<TransactionsService>(Lifestyle.Singleton);
            Container.Register<TransactionsStatusService>(Lifestyle.Singleton);
            Container.Register<VBalancebyCategoryService>(Lifestyle.Singleton);
            Container.Register<VPortfolioService>(Lifestyle.Singleton);
        }


    }
}
