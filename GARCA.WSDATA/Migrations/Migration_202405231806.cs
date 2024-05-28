using Dapper;
using GARCA.Utils.Logging;
using GARCA.wsData.Migrations.Seeder;
using GARCA.wsData.Repositories;

namespace wsData.Migrations
{
    public class Migration_202405231806
    {
        public  void Do()
        {
            try
            {
                using (var connection = DBContext.OpenConnection(true))
                {
                    connection.Execute(@"

                        DROP PROCEDURE UpdateBalancebyDate;
                    ");                                                

                    connection.Execute(@"
                        CREATE PROCEDURE UpdateBalancebyDate(IN s_date DATE)
                        BEGIN   
 	
	                        DECLARE done INT DEFAULT 0;    
                            DECLARE s_accountid INT;
    

	                        DECLARE cur CURSOR FOR 	
		                        SELECT accountid
                                FROM Transactions t
                                WHERE date>=s_date
                                group by accountid;
    
                            DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;

	                        OPEN cur;
    
                            read_loop: LOOP
                                FETCH cur INTO s_accountid;
        
                                IF done THEN
                                    LEAVE read_loop;
                                END IF;
        
                                CALL UpdateBalancebyDateAccount(s_date,s_accountid);
     
 	                        END LOOP;
    
	                        CLOSE cur;
	
                        END;
                    ");                                                

                    connection.Execute(@"

                        CREATE PROCEDURE UpdateBalancebyDateAccount(IN s_date DATE, IN s_accountid INT)
                        BEGIN   
 	
	                        DECLARE done INT DEFAULT 0;    
                            DECLARE c_id INT;
                            DECLARE c_amountIn DECIMAL(10, 2);
                            DECLARE c_amountOut DECIMAL(10, 2);   
                            DECLARE c_balance DECIMAL(10, 2) DEFAULT 0;
                            DECLARE p_balance DECIMAL(10, 2) DEFAULT 0;                                                     
					                           
	                        DECLARE cur CURSOR FOR 	
		                        SELECT id, amountIn, amountOut
                                FROM Transactions t
                                WHERE accountid = s_accountid and date >= s_date
                                ORDER BY orden;
    
                            DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
                           
                           	SELECT IFNULL(SUM(amountIn - amountOut), 0) INTO p_balance
						    FROM Transactions
						    WHERE accountid = s_accountid AND date < s_date;

	                        OPEN cur;
    
                            read_loop: LOOP
                                FETCH cur INTO c_id, c_amountIn, c_amountOut;
        
                                IF done THEN
                                    LEAVE read_loop;
                                END IF;
        
                                SET c_balance = p_balance + (c_amountIn - c_amountOut);
  		                        SET p_balance = c_balance;
           	                        
  	                            UPDATE Transactions 
                                SET balance = c_balance 
                                WHERE id = c_id;
     
     
 	                        END LOOP;
    
	                        CLOSE cur;
	
                        END;

                    ");                                                

                    connection.Execute(@"
                        INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202405231806', '5.0');
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
