using Dapper;
using GARCA.Utils.Logging;
using GARCA.wsData.Migrations.Seeder;
using GARCA.wsData.Repositories;


namespace wsData.Migrations
{
    public class Migration_202405042356
    {
        public  void Do()
        {
            try
            {
                using (var connection = DBContext.OpenConnection(true))
                {
                    connection.Execute(@"

                        CREATE TRIGGER TransactionsReminders_UT
                         BEFORE UPDATE ON TransactionsReminders 
                         FOR EACH ROW
                         BEGIN

                             IF NEW.AmountIn IS NULL THEN
                                 SET NEW.AmountIn = 0;
                             END IF;
    
                             IF NEW.AmountOut IS NULL THEN
                                 SET NEW.AmountOut = 0;
                             END IF;
                         END;
 
                        ");

                        connection.Execute(@"

                        CREATE TRIGGER TransactionsReminders_IT
                        BEFORE INSERT ON TransactionsReminders
                        FOR EACH ROW
                        BEGIN

                            IF NEW.AmountIn IS NULL THEN
                                SET NEW.AmountIn = 0;
                            END IF;
    
                            IF NEW.AmountOut IS NULL THEN
                                SET NEW.AmountOut = 0;
                            END IF;
                        END;
                        ");

                        connection.Execute(@"

                         CREATE TRIGGER Splits_UT
                         BEFORE UPDATE ON Splits 
                         FOR EACH ROW
                         BEGIN

                             IF NEW.AmountIn IS NULL THEN
                                 SET NEW.AmountIn = 0;
                             END IF;
    
                             IF NEW.AmountOut IS NULL THEN
                                 SET NEW.AmountOut = 0;
                             END IF;
                         END;
 
                        ");

                        connection.Execute(@"

                        CREATE TRIGGER Splits_IT
                        BEFORE INSERT ON Splits
                        FOR EACH ROW
                        BEGIN

                            IF NEW.AmountIn IS NULL THEN
                                SET NEW.AmountIn = 0;
                            END IF;
    
                            IF NEW.AmountOut IS NULL THEN
                                SET NEW.AmountOut = 0;
                            END IF;
                        END;
                        ");

                        connection.Execute(@"

                        CREATE TRIGGER SplitsReminders_UT
                         BEFORE UPDATE ON SplitsReminders 
                         FOR EACH ROW
                         BEGIN

                             IF NEW.AmountIn IS NULL THEN
                                 SET NEW.AmountIn = 0;
                             END IF;
    
                             IF NEW.AmountOut IS NULL THEN
                                 SET NEW.AmountOut = 0;
                             END IF;
                         END;
 
                        ");

                        connection.Execute(@"

                        CREATE TRIGGER SplitsReminders_IT
                        BEFORE INSERT ON SplitsReminders
                        FOR EACH ROW
                        BEGIN

                            IF NEW.AmountIn IS NULL THEN
                                SET NEW.AmountIn = 0;
                            END IF;
    
                            IF NEW.AmountOut IS NULL THEN
                                SET NEW.AmountOut = 0;
                            END IF;
                        END;

                        ");

                        connection.Execute(@"

                        CREATE PROCEDURE UpdateBalancebyId(IN Tid int)
                        BEGIN   
 
                             DECLARE Tdate datetime;

                            SELECT date INTO Tdate
	                        FROM Transactions t 
	                        where t.id = Tid;

	                        call UpdateBalancebyDate(Tdate);

                        END

                        ");

                        connection.Execute(@"

                        CREATE TRIGGER Splits_Update_Transaction_IT
                            AFTER INSERT ON Splits
                            FOR EACH ROW
                            BEGIN
    
	                            UPDATE Transactions t
	                            LEFT JOIN (
	                                SELECT transactionid, 
	                                       SUM(amountIn) AS total_amountIn,
	                                       SUM(amountOut) AS total_amountOut
	                                FROM Splits
	                                GROUP BY transactionid
	                            ) s ON t.id = s.transactionid
	                            SET t.amountIn = IFNULL(s.total_amountIn, 0),
	                                t.amountOut = IFNULL(s.total_amountOut, 0),
	                                t.categoryid = CASE
			                                            WHEN s.transactionid IS NULL THEN 0	                        
			                                            ELSE -1
		                                            END
	                            WHERE t.id = NEW.transactionid;
	
                            END
                        ");

                        connection.Execute(@"

                            CREATE TRIGGER Splits_Update_Transaction_UT
                            AFTER UPDATE ON Splits
                            FOR EACH ROW
                            BEGIN
                               IF NEW.amountIn <> OLD.amountIn OR NEW.amountOut <> OLD.amountOut THEN
		                            UPDATE Transactions t
		                            LEFT JOIN (
		                                SELECT transactionid, 
		                                       SUM(amountIn) AS total_amountIn,
		                                       SUM(amountOut) AS total_amountOut
		                                FROM Splits
		                                GROUP BY transactionid
		                            ) s ON t.id = s.transactionid
		                            SET t.amountIn = IFNULL(s.total_amountIn, 0),
		                                t.amountOut = IFNULL(s.total_amountOut, 0),
		                                t.categoryid = CASE
			                                                WHEN s.transactionid IS NULL THEN 0	                        
			                                                ELSE -1
		                                                END
		                            WHERE t.id = NEW.transactionid;
	                            END IF;
                            END

                        ");

                        connection.Execute(@"

                            CREATE TRIGGER Splits_Update_Transaction_DT
                            AFTER DELETE ON Splits
                            FOR EACH ROW
                            BEGIN
    
	                            UPDATE Transactions t
	                            LEFT JOIN (
	                                SELECT transactionid, 
	                                       SUM(amountIn) AS total_amountIn,
	                                       SUM(amountOut) AS total_amountOut
	                                FROM Splits
	                                GROUP BY transactionid
	                            ) s ON t.id = s.transactionid
	                            SET t.amountIn = IFNULL(s.total_amountIn, 0),
	                                t.amountOut = IFNULL(s.total_amountOut, 0),
	                                t.categoryid = CASE
			                                            WHEN s.transactionid IS NULL THEN 0	                        
			                                            ELSE -1
		                                            END
	                            WHERE t.id = OLD.transactionid;
	
                            END
                        ");

                    connection.Execute(@"
                        INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202405042356', '5.0');
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
