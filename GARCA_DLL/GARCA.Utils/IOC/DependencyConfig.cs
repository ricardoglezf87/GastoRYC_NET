using GARCA.BO.Services;
using SimpleInjector;

namespace GARCA.Utils.IOC
{
    internal static class DependencyConfig
    {
        private static readonly Container Container;
        public static AccountsService AccountsService => Container.GetInstance<AccountsService>();
        public static AccountsTypesService AccountsTypesService => Container.GetInstance<AccountsTypesService>();
        public static CategoriesService CategoriesService => Container.GetInstance<CategoriesService>();
        public static CategoriesTypesService CategoriesTypesService => Container.GetInstance<CategoriesTypesService>();
        public static DateCalendarService DateCalendarService => Container.GetInstance<DateCalendarService>();
        public static ExpirationsRemindersService ExpirationsRemindersService => Container.GetInstance<ExpirationsRemindersService>();
        public static ForecastsChartService ForecastsChartService => Container.GetInstance<ForecastsChartService>();
        public static InvestmentProductsPricesService InvestmentProductsPricesService => Container.GetInstance<InvestmentProductsPricesService>();
        public static InvestmentProductsService InvestmentProductsService => Container.GetInstance<InvestmentProductsService>();
        public static InvestmentProductsTypesService InvestmentProductsTypesService => Container.GetInstance<InvestmentProductsTypesService>();
        public static PeriodsRemindersService PeriodsReminderService => Container.GetInstance<PeriodsRemindersService>();
        public static PersonsService PersonsService => Container.GetInstance<PersonsService>();
        public static RycContextService RycContextService => Container.GetInstance<RycContextService>();
        public static SplitsRemindersService SplitsRemindersService => Container.GetInstance<SplitsRemindersService>();
        public static SplitsService SplitsService => Container.GetInstance<SplitsService>();
        public static SplitsArchivedService SplitsArchivedService => Container.GetInstance<SplitsArchivedService>();
        public static TagsService TagsService => Container.GetInstance<TagsService>();
        public static TransactionsRemindersService TransactionsRemindersService => Container.GetInstance<TransactionsRemindersService>();
        public static TransactionsService TransactionsService => Container.GetInstance<TransactionsService>();
        public static TransactionsArchivedService TransactionsArchivedService => Container.GetInstance<TransactionsArchivedService>();
        public static TransactionsStatusService TransactionsStatusService => Container.GetInstance<TransactionsStatusService>();
        public static VBalancebyCategoryService IvBalancebyCategoryService => Container.GetInstance<VBalancebyCategoryService>();
        public static VPortfolioService IvPortfolioService => Container.GetInstance<VPortfolioService>();

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
            Container.Register<SplitsArchivedService>(Lifestyle.Singleton);
            Container.Register<TagsService>(Lifestyle.Singleton);
            Container.Register<TransactionsRemindersService>(Lifestyle.Singleton);
            Container.Register<TransactionsService>(Lifestyle.Singleton);
            Container.Register<TransactionsArchivedService>(Lifestyle.Singleton);
            Container.Register<TransactionsStatusService>(Lifestyle.Singleton);
            Container.Register<VBalancebyCategoryService>(Lifestyle.Singleton);
            Container.Register<VPortfolioService>(Lifestyle.Singleton);
        }
    }
}
