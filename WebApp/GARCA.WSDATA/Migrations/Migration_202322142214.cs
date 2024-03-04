using Dapper;
using GARCA.wsData.Managers;


namespace wsData.Migrations
{
    public class Migration_202322142214
    {
        public async Task Do()
        {
            await dbContext.OpenConnection().ExecuteAsync(@"

                    DROP TABLE dateCalendar;

                    CREATE TABLE dateCalendar (
                        ""id"" INTEGER PRIMARY KEY,
                        ""day"" INTEGER,
                        ""month"" INTEGER,
                        ""year"" INTEGER,
                        ""date"" TEXT
                    ); 

                ");

            await dbContext.OpenConnection().ExecuteAsync(@"

                INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202322142214', '4.0');

                ");

        }
    }
}
