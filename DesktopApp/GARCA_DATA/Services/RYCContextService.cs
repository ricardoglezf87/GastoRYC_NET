using GARCA.Data.Managers;
using Microsoft.Data.Sqlite;

namespace GARCA.Data.Services
{
    public class RycContextService
    {
        private readonly RycContextManager contextManager;

        public RycContextService()
        {
            contextManager = new RycContextManager();
        }

        public SqliteConnection getConnection()
        {
            return new SqliteConnection("Data Source=Data\\rycBBDD_PRE.db");
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
