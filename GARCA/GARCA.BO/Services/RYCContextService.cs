using GARCA.DAO.Managers;

namespace GARCA.BO.Services
{
    public class RYCContextService
    {
        private readonly RYCContextManager contextManager;

        public RYCContextService()
        {
            contextManager = new RYCContextManager();
        }

        public void migrateDataBase()
        {
            contextManager.migrateDataBase();
        }

        public void makeBackup()
        {
            contextManager.makeBackup();
        }
    }
}
