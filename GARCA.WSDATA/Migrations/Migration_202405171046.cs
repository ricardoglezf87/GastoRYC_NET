using Dapper;
using GARCA.Utils.Logging;
using GARCA.wsData.Migrations.Seeder;
using GARCA.wsData.Repositories;


namespace wsData.Migrations
{
    public class Migration_202405171046
    {
        public  void Do()
        {
            try
            {
                using (var connection = DBContext.OpenConnection(true))
                {
                        connection.Execute(@"

                        CREATE PROCEDURE UpdateTransactionWithSplit(IN Tid INT)
                        BEGIN
      
                            UPDATE Transactions t
                            LEFT JOIN (
                                SELECT transactionid, 
                                       ROUND(SUM(amountIn),2) AS total_amountIn,
                                       ROUND(SUM(amountOut),2) AS total_amountOut
                                FROM Splits
                                GROUP BY transactionid
                            ) s ON t.id = s.transactionid
                            SET t.amountIn = IFNULL(s.total_amountIn, 0),
                                t.amountOut = IFNULL(s.total_amountOut, 0),
                                t.categoryid = CASE
                                                    WHEN s.transactionid IS NULL THEN 0	                        
                                                    ELSE -1
                                                END
                            WHERE t.id = Tid;
      
                        END
 
                        ");                        

                        connection.Execute(@"

                        DROP TRIGGER GARCA_TEST.Splits_Update_Transaction_IT;
                        
                        ");

                        connection.Execute(@"

                        DROP TRIGGER GARCA_TEST.Splits_Update_Transaction_UT;
                        
                        ");

                        connection.Execute(@"

                        DROP TRIGGER GARCA_TEST.Splits_Update_Transaction_DT;
                        
                        ");

                    connection.Execute(@"
                        INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202405171046', '5.0');
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
