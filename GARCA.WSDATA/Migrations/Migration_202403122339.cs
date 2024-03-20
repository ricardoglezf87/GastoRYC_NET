using Dapper;
using GARCA.Utils.Logging;
using GARCA.wsData.Migrations.Seeder;
using GARCA.wsData.Repositories;


namespace wsData.Migrations
{
    public class Migration_202403122339
    {
        public  void Do()
        {
            try
            {
                CategoriesTypesSeed.Do();
                AccountsTypesSeed.Do();
                InvestmentProductTypesSeed.Do();
                PeriodsRemindersSeed.Do();
                TransactionsStatusSeed.Do();

                dbContext.OpenConnection(true).Execute(@"

                INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202403122339', '5.0');

                ");
            }
            catch(Exception ex)
            {
                Log.LogError(ex.Message);
            }
        }
    }
}
