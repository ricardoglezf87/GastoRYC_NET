using Dapper;
using GARCA.Utils.Logging;
using GARCA_UTIL.Exceptions;
using System.Reflection;



namespace GARCA.wsData.Repositories
{
    public static class MigrationRepository
    {

        public static void CleanDataBase()
        {
            try
            {
                dbContext.OpenConnection(true).Execute(@"
                    DROP TABLE GARCA_TEST.ExpirationsReminders;
                    DROP TABLE GARCA_TEST.SplitsArchived;
                    DROP TABLE GARCA_TEST.SplitsReminders;
                    DROP TABLE GARCA_TEST.TransactionsReminders;
                    DROP TABLE GARCA_TEST.Splits;
                    DROP TABLE GARCA_TEST.Transactions;
                    DROP TABLE GARCA_TEST.InvestmentProductsPrices;
                    DROP TABLE GARCA_TEST.TransactionsArchived;
                    DROP TABLE GARCA_TEST.Accounts;
                    DROP TABLE GARCA_TEST.AccountsTypes;
                    DROP TABLE GARCA_TEST.DateCalendar;
                    DROP TABLE GARCA_TEST.InvestmentProducts;
                    DROP TABLE GARCA_TEST.InvestmentProductsTypes;
                    DROP TABLE GARCA_TEST.MigrationsHistory;
                    DROP TABLE GARCA_TEST.PeriodsReminders;
                    DROP TABLE GARCA_TEST.Persons;
                    DROP TABLE GARCA_TEST.Tags;
                    DROP TABLE GARCA_TEST.TransactionsStatus;
                    DROP TABLE GARCA_TEST.Categories;
                    DROP TABLE GARCA_TEST.CategoriesTypes;
                ");



                dbContext.OpenConnection(true).Execute(@"

                INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202403030902', '5.0');

                ");
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
            }
        }


        public static void Migrate()
        {
            dbContext.OpenConnection(true).Execute(@"

                -- MigrationsHistory definition

               CREATE TABLE IF NOT EXISTS `MigrationsHistory` (
                  `MigrationId` text DEFAULT NULL,
                  `ProductVersion` varchar(100) DEFAULT NULL
                );
            ");


            Type[] clases = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Namespace == "wsData.Migrations" && t.IsClass && !t.IsNested)
                .ToArray();

            foreach (Type clase in clases)
            {
                if (IsMigrateFeature(clase.Name))
                {
                    continue;
                }

                object? instancia = Activator.CreateInstance(clase);

                MethodInfo? metodoDo = clase.GetMethod("Do");

                if (metodoDo != null)
                {
                     metodoDo.Invoke(instancia, null);
                }
                else
                {
                    throw new MigrationException($"No se cuentra el metodo Do en la clase {clase.Name}");
                }
            }
        }

        public static bool IsMigrateFeature(string feature)
        {
            return Convert.ToInt32(dbContext.OpenConnection().ExecuteScalar($"Select count(*) from MigrationsHistory where MigrationId='{feature}'")) != 0;
        }
    }
}
