using GARCA.View.Services;
using SimpleInjector;

namespace GARCA.Data.IOC
{
    public static class DependencyConfigView
    {
        private static readonly Container Container;
        public static AccountsServiceView AccountsServiceView => Container.GetInstance<AccountsServiceView>();
        public static AccountsTypesServiceView AccountsTypesServiceView => Container.GetInstance<AccountsTypesServiceView>();
        public static CategoriesServiceView CategoriesServiceView => Container.GetInstance<CategoriesServiceView>();
        public static CategoriesTypesServiceView CategoriesTypesServiceView => Container.GetInstance<CategoriesTypesServiceView>();
        public static DateCalendarServiceView DateCalendarServiceView => Container.GetInstance<DateCalendarServiceView>();
        public static ExpirationsRemindersServiceView ExpirationsRemindersServiceView => Container.GetInstance<ExpirationsRemindersServiceView>();
        public static ForecastsChartServiceView ForecastsChartServiceView => Container.GetInstance<ForecastsChartServiceView>();
        public static InvestmentProductsPricesServiceView InvestmentProductsPricesServiceView => Container.GetInstance<InvestmentProductsPricesServiceView>();
        public static InvestmentProductsServiceView InvestmentProductsServiceView => Container.GetInstance<InvestmentProductsServiceView>();
        public static InvestmentProductsTypesServiceView InvestmentProductsTypesServiceView => Container.GetInstance<InvestmentProductsTypesServiceView>();
        public static PeriodsRemindersServiceView PeriodsReminderServiceView => Container.GetInstance<PeriodsRemindersServiceView>();
        public static PersonsServiceView PersonsServiceView => Container.GetInstance<PersonsServiceView>();
        public static RycContextServiceView RycContextServiceView => Container.GetInstance<RycContextServiceView>();
        public static SplitsRemindersServiceView SplitsRemindersServiceView => Container.GetInstance<SplitsRemindersServiceView>();
        public static SplitsServiceView SplitsServiceView => Container.GetInstance<SplitsServiceView>();
        public static SplitsArchivedServiceView SplitsArchivedServiceView => Container.GetInstance<SplitsArchivedServiceView>();
        public static TagsServiceView TagsServiceView => Container.GetInstance<TagsServiceView>();
        public static TransactionsRemindersServiceView TransactionsRemindersServiceView => Container.GetInstance<TransactionsRemindersServiceView>();
        public static TransactionsServiceView TransactionsServiceView => Container.GetInstance<TransactionsServiceView>();
        public static TransactionsArchivedServiceView TransactionsArchivedServiceView => Container.GetInstance<TransactionsArchivedServiceView>();
        public static TransactionsStatusServiceView TransactionsStatusServiceView => Container.GetInstance<TransactionsStatusServiceView>();
        public static VBalancebyCategoryServiceView IvBalancebyCategoryServiceView => Container.GetInstance<VBalancebyCategoryServiceView>();
        public static VPortfolioServiceView IvPortfolioServiceView => Container.GetInstance<VPortfolioServiceView>();

        static DependencyConfigView()
        {
            Container = new Container();
            RegisterDependencies();
        }

        private static void RegisterDependencies()
        {
            Container.Register<AccountsServiceView>(Lifestyle.Singleton);
            Container.Register<AccountsTypesServiceView>(Lifestyle.Singleton);
            Container.Register<CategoriesServiceView>(Lifestyle.Singleton);
            Container.Register<CategoriesTypesServiceView>(Lifestyle.Singleton);
            Container.Register<DateCalendarServiceView>(Lifestyle.Singleton);
            Container.Register<ExpirationsRemindersServiceView>(Lifestyle.Singleton);
            Container.Register<ForecastsChartServiceView>(Lifestyle.Singleton);
            Container.Register<InvestmentProductsPricesServiceView>(Lifestyle.Singleton);
            Container.Register<InvestmentProductsServiceView>(Lifestyle.Singleton);
            Container.Register<InvestmentProductsTypesServiceView>(Lifestyle.Singleton);
            Container.Register<PeriodsRemindersServiceView>(Lifestyle.Singleton);
            Container.Register<PersonsServiceView>(Lifestyle.Singleton);
            Container.Register<RycContextServiceView>(Lifestyle.Singleton);
            Container.Register<SplitsRemindersServiceView>(Lifestyle.Singleton);
            Container.Register<SplitsServiceView>(Lifestyle.Singleton);
            Container.Register<SplitsArchivedServiceView>(Lifestyle.Singleton);
            Container.Register<TagsServiceView>(Lifestyle.Singleton);
            Container.Register<TransactionsRemindersServiceView>(Lifestyle.Singleton);
            Container.Register<TransactionsServiceView>(Lifestyle.Singleton);
            Container.Register<TransactionsArchivedServiceView>(Lifestyle.Singleton);
            Container.Register<TransactionsStatusServiceView>(Lifestyle.Singleton);
            Container.Register<VBalancebyCategoryServiceView>(Lifestyle.Singleton);
            Container.Register<VPortfolioServiceView>(Lifestyle.Singleton);
        }

    }
}
