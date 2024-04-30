using GARCA.Data.Services;
using SimpleInjector;

namespace GARCA.Data.IOC
{
    public static class DependencyConfig
    {
        private static readonly Container Container;
        public static AccountsService iAccountsService => Container.GetInstance<AccountsService>();
        public static AccountsTypesService iAccountsTypesService => Container.GetInstance<AccountsTypesService>();
        public static CategoriesService iCategoriesService => Container.GetInstance<CategoriesService>();
        public static CategoriesTypesService iCategoriesTypesService => Container.GetInstance<CategoriesTypesService>();        
        public static ExpirationsRemindersService iExpirationsRemindersService => Container.GetInstance<ExpirationsRemindersService>();        
        public static InvestmentProductsPricesService iInvestmentProductsPricesService => Container.GetInstance<InvestmentProductsPricesService>();
        public static InvestmentProductsService iInvestmentProductsService => Container.GetInstance<InvestmentProductsService>();
        public static InvestmentProductsTypesService iInvestmentProductsTypesService => Container.GetInstance<InvestmentProductsTypesService>();
        public static PeriodsRemindersService iPeriodsReminderService => Container.GetInstance<PeriodsRemindersService>();
        public static PersonsService iPersonsService => Container.GetInstance<PersonsService>();
        public static RycContextService iRycContextService => Container.GetInstance<RycContextService>();
        public static SplitsRemindersService iSplitsRemindersService => Container.GetInstance<SplitsRemindersService>();
        public static SplitsService iSplitsService => Container.GetInstance<SplitsService>();
        public static TagsService iTagsService => Container.GetInstance<TagsService>();
        public static TransactionsRemindersService iTransactionsRemindersService => Container.GetInstance<TransactionsRemindersService>();
        public static TransactionsService iTransactionsService => Container.GetInstance<TransactionsService>();
        public static TransactionsStatusService iTransactionsStatusService => Container.GetInstance<TransactionsStatusService>();
        public static VPortfolioService iVPortfolioService => Container.GetInstance<VPortfolioService>();

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
            Container.Register<ExpirationsRemindersService>(Lifestyle.Singleton);            
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
            Container.Register<VPortfolioService>(Lifestyle.Singleton);
        }
    }
}
