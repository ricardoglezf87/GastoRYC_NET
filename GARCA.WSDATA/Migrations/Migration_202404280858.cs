using Dapper;
using GARCA.Utils.Logging;
using GARCA.wsData.Migrations.Seeder;
using GARCA.wsData.Repositories;


namespace wsData.Migrations
{
    public class Migration_202404280858
    {
        public  void Do()
        {
            try
            {
                using (var connection = dbContext.OpenConnection(true))
                {
                    connection.Execute(@"
                        CREATE PROCEDURE UpdatePersonsCategoriesId (IN person_id INT)
                        BEGIN
                            DECLARE category_id INT;
    
                            START TRANSACTION;

                            SELECT categoryid INTO category_id
	                        FROM (
	                            SELECT categoryid, COUNT(categoryid) AS repetition_count
	                            FROM Transactions 
	                            WHERE personid = person_id
	                            GROUP BY categoryid
	                            ORDER BY repetition_count DESC
	                            LIMIT 1
	                        ) AS A;

                            IF category_id IS NOT NULL THEN
                                UPDATE Persons SET categoryid = category_id WHERE id = person_id;
                            END IF;

                            COMMIT;
                        END
                    ");

                    connection.Execute(@"
                        CREATE PROCEDURE UpdateBalancebyDate(IN p_transaction_date DATE)
                        BEGIN   
 
                            UPDATE Transactions t
                            SET balance = (
                                SELECT ROUND(SUM(t2.amountIn - t2.amountOut), 2)
                                FROM Transactions t2
                                WHERE t2.accountid = t.accountid
                                    AND t2.orden <= t.orden
                            )
                            WHERE date >= p_transaction_date;
                        END
                    ");

                    connection.Execute(@"
                        CREATE TRIGGER Transactions_IT
                        BEFORE INSERT ON Transactions
                        FOR EACH ROW
                        BEGIN

                            IF NEW.AmountIn IS NULL THEN
                                SET NEW.AmountIn = 0;
                            END IF;
    
                            IF NEW.AmountOut IS NULL THEN
                                SET NEW.AmountOut = 0;
                            END IF;

                            SET NEW.orden = CAST(CONCAT(
                                                    YEAR(NEW.Date),
                                                    LPAD(MONTH(NEW.Date), 2, '0'),
                                                    LPAD(DAY(NEW.Date), 2, '0'),
                                                    LPAD(NEW.id, 6, '0'),
                                                    IF(NEW.AmountIn != 0, '1', '0')
                                                ) AS DECIMAL(20,0));
                        END;
                    ");

                    connection.Execute(@"
                        CREATE TRIGGER Transactions_UT
                        BEFORE UPDATE ON Transactions
                        FOR EACH ROW
                        BEGIN

                            IF NEW.AmountIn IS NULL THEN
                                SET NEW.AmountIn = 0;
                            END IF;
    
                            IF NEW.AmountOut IS NULL THEN
                                SET NEW.AmountOut = 0;
                            END IF;

                            SET NEW.orden = CAST(CONCAT(
                                                    YEAR(NEW.Date),
                                                    LPAD(MONTH(NEW.Date), 2, '0'),
                                                    LPAD(DAY(NEW.Date), 2, '0'),
                                                    LPAD(NEW.id, 6, '0'),
                                                    IF(NEW.AmountIn != 0, '1', '0')
                                                ) AS DECIMAL(20,0));
                        END;
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
