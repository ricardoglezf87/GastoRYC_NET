using GARCA.DAO.Managers;

namespace GARCA.BO.Services
{
    public class RycContextService
    {
        private readonly RycContextManager contextManager;

        public RycContextService()
        {
            contextManager = new RycContextManager();
        }

        public void MigrateDataBase()
        {
            contextManager.MigrateDataBase();
        }

        public void MakeBackup()
        {
            contextManager.MakeBackup();
        }
    }
}
