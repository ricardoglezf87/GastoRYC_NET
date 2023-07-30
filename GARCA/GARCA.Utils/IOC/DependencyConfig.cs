using GARCA.BO.Services;
using SimpleInjector;

namespace GARCA.Utils.IOC
{
    public static class DependencyConfig
    {
        private static readonly Container Container;

        public static AccountsService IAccountsService => Container.GetInstance<AccountsService>();

        public static AccountsTypesService IAccountsTypesService => Container.GetInstance<AccountsTypesService>();

        public static CategoriesService ICategoriesService => Container.GetInstance<CategoriesService>();

        public static CategoriesTypesService ICategoriesTypesService => Container.GetInstance<CategoriesTypesService>();

        public static DateCalendarService IDateCalendarService => Container.GetInstance<DateCalendarService>();

        public static ExpirationsRemindersService IExpirationsRemindersService => Container.GetInstance<ExpirationsRemindersService>();

        public static ForecastsChartService IForecastsChartService => Container.GetInstance<ForecastsChartService>();

        public static InvestmentProductsPricesService IInvestmentProductsPricesService => Container.GetInstance<InvestmentProductsPricesService>();

        public static InvestmentProductsService IInvestmentProductsService => Container.GetInstance<InvestmentProductsService>();

        public static InvestmentProductsTypesService IInvestmentProductsTypesService => Container.GetInstance<InvestmentProductsTypesService>();

        public static PeriodsRemindersService IPeriodsReminderService => Container.GetInstance<PeriodsRemindersService>();

        public static PersonsService IPersonsService => Container.GetInstance<PersonsService>();

        public static RycContextService IRycContextService => Container.GetInstance<RycContextService>();

        public static SplitsRemindersService ISplitsRemindersService => Container.GetInstance<SplitsRemindersService>();

        public static SplitsService ISplitsService => Container.GetInstance<SplitsService>();

        public static TagsService ITagsService => Container.GetInstance<TagsService>();
        public static TransactionsRemindersService ITransactionsRemindersService => Container.GetInstance<TransactionsRemindersService>();
        public static TransactionsService ITransactionsService => Container.GetInstance<TransactionsService>();
        public static TransactionsStatusService ITransactionsStatusService => Container.GetInstance<TransactionsStatusService>();
        public static VBalancebyCategoryService IVBalancebyCategoryService => Container.GetInstance<VBalancebyCategoryService>();
        public static VPortfolioService IVPortfolioService => Container.GetInstance<VPortfolioService>();

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
