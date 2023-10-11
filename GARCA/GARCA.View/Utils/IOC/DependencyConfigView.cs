using GARCA.BO.Services;
using GARCA.View.Services;
using SimpleInjector;

namespace GARCA.Utils.IOC
{
    public static class DependencyConfigView
    {
        private static readonly Container Container;
        public static AccountsServiceView AccountsServiceView => Container.GetInstance<AccountsServiceView>();
        public static TransactionsArchivedServiceView TransactionsArchivedServiceView => Container.GetInstance<TransactionsArchivedServiceView>();

        static DependencyConfigView()
        {
            Container = new Container();
            RegisterDependencies();
        }

        private static void RegisterDependencies()
        {
            Container.Register<AccountsServiceView>(Lifestyle.Singleton);
            Container.Register<TransactionsArchivedServiceView>(Lifestyle.Singleton);
        }


    }
}
