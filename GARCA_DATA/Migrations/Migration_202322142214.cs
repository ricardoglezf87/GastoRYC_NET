using Dapper;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Migrations
{
    public class Migration_202322142214
    {
        public async Task Do()
        {
            using (var connection = iRycContextService.getConnection())
            {

                await connection.ExecuteAsync(@"

                    DROP TABLE dateCalendar;

                    CREATE TABLE dateCalendar (
                        ""id"" INTEGER PRIMARY KEY,
                        ""day"" INTEGER,
                        ""month"" INTEGER,
                        ""year"" INTEGER,
                        ""date"" TEXT
                    ); 

                ");

                await connection.ExecuteAsync(@"

                    INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202322142214', '4.0');

                ");
            }

        }
    }
}
