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
                using (var connection = dbContext.OpenConnection(true))
                {
                   connection.Execute(@"
                        DROP TABLE IF EXISTS GARCA_TEST.ExpirationsReminders;
                        DROP TABLE IF EXISTS GARCA_TEST.SplitsReminders;
                        DROP TABLE IF EXISTS GARCA_TEST.TransactionsReminders;
                        DROP TABLE IF EXISTS GARCA_TEST.Splits;
                        DROP TABLE IF EXISTS GARCA_TEST.Transactions;
                        DROP TABLE IF EXISTS GARCA_TEST.InvestmentProductsPrices;
                        DROP TABLE IF EXISTS GARCA_TEST.Accounts;
                        DROP TABLE IF EXISTS GARCA_TEST.AccountsTypes;
                        DROP TABLE IF EXISTS GARCA_TEST.DateCalendar;
                        DROP TABLE IF EXISTS GARCA_TEST.InvestmentProducts;
                        DROP TABLE IF EXISTS GARCA_TEST.InvestmentProductsTypes;
                        DROP TABLE IF EXISTS GARCA_TEST.MigrationsHistory;
                        DROP TABLE IF EXISTS GARCA_TEST.PeriodsReminders;
                        DROP TABLE IF EXISTS GARCA_TEST.Persons;
                        DROP TABLE IF EXISTS GARCA_TEST.Tags;
                        DROP TABLE IF EXISTS GARCA_TEST.TransactionsStatus;
                        DROP TABLE IF EXISTS GARCA_TEST.Categories;
                        DROP TABLE IF EXISTS GARCA_TEST.CategoriesTypes;

                        DROP PROCEDURE IF EXISTS GARCA_TEST.UpdateBalancebyDate;
                        DROP PROCEDURE IF EXISTS GARCA_TEST.UpdateBalancebyId;
                        DROP PROCEDURE IF EXISTS GARCA_TEST.UpdatePersonsCategoriesId;
                        DROP PROCEDURE IF EXISTS GARCA_TEST.UpdateTranfer;
                        DROP PROCEDURE IF EXISTS GARCA_TEST.UpdateTranferSplit;

                        DROP TRIGGER IF EXISTS GARCA_TEST.Splits_IT;
                        DROP TRIGGER IF EXISTS GARCA_TEST.Splits_Update_Transaction_IT;
                        DROP TRIGGER IF EXISTS GARCA_TEST.Splits_UT;
                        DROP TRIGGER IF EXISTS GARCA_TEST.Splits_Update_Transaction_UT;
                        DROP TRIGGER IF EXISTS GARCA_TEST.SplitsReminders_IT;
                        DROP TRIGGER IF EXISTS GARCA_TEST.SplitsReminders_UT;
                        DROP TRIGGER IF EXISTS GARCA_TEST.TransactionsReminders_IT;
                        DROP TRIGGER IF EXISTS GARCA_TEST.TransactionsReminders_UT;
                        DROP TRIGGER IF EXISTS GARCA_TEST.Splits_Update_Transaction_DT;
                        DROP TRIGGER IF EXISTS GARCA_TEST.Transactions_IT;
                        DROP TRIGGER IF EXISTS GARCA_TEST.Transactions_UT;
                        DROP TRIGGER IF EXISTS GARCA_TEST.Accounts_Categories_IT;
                        DROP TRIGGER IF EXISTS GARCA_TEST.Accounts_Categories_UT;
                        DROP TRIGGER IF EXISTS GARCA_TEST.Accounts_Categories_DT;
                    ");

                }                
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
            }
        }


        public static void Migrate()
        {
            using (var connection = dbContext.OpenConnection(true))
            {
                connection.Execute(@"
                    -- MigrationsHistory definition

                   CREATE TABLE IF NOT EXISTS `MigrationsHistory` (
                      `MigrationId` text DEFAULT NULL,
                      `ProductVersion` varchar(100) DEFAULT NULL
                    );
                ");
            }

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
            using (var connection = dbContext.OpenConnection())
            {
                return Convert.ToInt32(connection.ExecuteScalar($"Select count(*) from MigrationsHistory where MigrationId='{feature}'")) != 0;
            }
        }
    }
}
