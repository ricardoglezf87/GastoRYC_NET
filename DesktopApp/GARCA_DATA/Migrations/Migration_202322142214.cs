using Dapper;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Migrations
{
    public class Migration_202322142214
    {
        public async Task Do()
        {
            await iRycContextService.getConnection().ExecuteAsync("ALTER TABLE dateCalendar RENAME COLUMN date TO id;");

            await iRycContextService.getConnection().ExecuteAsync("INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202322142214', '4.0');");
        }
    }
}
