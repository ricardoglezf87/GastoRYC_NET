using GARCA.DAO.Managers;

namespace GARCA.BO.Services
{
    public class RYCContextService
    {
        private readonly RYCContextManager contextManager;
        private static RYCContextService? _instance;
        private static readonly object _lock = new();

        public static RYCContextService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new RYCContextService();
                    }
                }
                return _instance;
            }
        }

        private RYCContextService()
        {
            contextManager = new();
        }

        public void migrateDataBase()
        {
            contextManager.migrateDataBase();
        }

        public void loadContext()
        {
            contextManager.loadContext();
        }

        public void makeBackup()
        {
            contextManager.makeBackup();
        }
    }
}
