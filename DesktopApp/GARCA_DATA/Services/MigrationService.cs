using GARCA.Data.Managers;

namespace GARCA.Data.Services
{
    public class MigrationService
    {
        private readonly MigrationManager migrationManager;

        public MigrationService()
        {
            migrationManager = new();
        }

        public async Task Migrate()
        {
            await migrationManager.Migrate();
        }
    }
}
