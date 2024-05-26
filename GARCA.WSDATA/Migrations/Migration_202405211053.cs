using Dapper;
using GARCA.Utils.Logging;
using GARCA.wsData.Migrations.Seeder;
using GARCA.wsData.Repositories;


namespace wsData.Migrations
{
    public class Migration_202405211053
    {
        public  void Do()
        {
            try
            {
                using (var connection = DBContext.OpenConnection(true))
                {
                        connection.Execute(@"

                         ALTER TABLE Transactions MODIFY COLUMN numShares decimal(20,8) DEFAULT NULL NULL;
                         ALTER TABLE Transactions MODIFY COLUMN pricesShares decimal(20,8) DEFAULT NULL NULL;
                         ALTER TABLE Transactions MODIFY COLUMN balance decimal(10,2) DEFAULT NULL NULL;
                         ALTER TABLE Transactions MODIFY COLUMN amountOut decimal(10,2) DEFAULT NULL NULL;
                         ALTER TABLE Transactions MODIFY COLUMN amountIn decimal(10,2) DEFAULT NULL NULL;
                            
                         ALTER TABLE Splits MODIFY COLUMN amountOut decimal(10,2) DEFAULT NULL NULL;
                         ALTER TABLE Splits MODIFY COLUMN amountIn decimal(10,2) DEFAULT NULL NULL;

                         ALTER TABLE TransactionsReminders MODIFY COLUMN amountOut decimal(10,2) DEFAULT NULL NULL;
                         ALTER TABLE TransactionsReminders MODIFY COLUMN amountIn decimal(10,2) DEFAULT NULL NULL;
                        
                         ALTER TABLE SplitsReminders MODIFY COLUMN amountOut decimal(10,2) DEFAULT NULL NULL;
                         ALTER TABLE SplitsReminders MODIFY COLUMN amountIn decimal(10,2) DEFAULT NULL NULL;

                         ALTER TABLE InvestmentProductsPrices MODIFY COLUMN prices decimal(20,8) DEFAULT NULL NULL;

                        ");                                                

                    connection.Execute(@"
                        INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202405211053', '5.0');
                    ");
                }
            }
            catch(Exception ex)
            {
                Log.LogError(ex.Message);
            }
        }
    }
}
