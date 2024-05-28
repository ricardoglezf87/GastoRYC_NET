using Dapper;
using GARCA.Utils.Logging;
using GARCA.wsData.Migrations.Seeder;
using GARCA.wsData.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace wsData.Migrations
{
    public class Migration_202405272326
    {
        public  void Do()
        {
            try
            {
                using (var connection = DBContext.OpenConnection(true))
                {
                    connection.Execute(@"

                        DROP PROCEDURE IF EXISTS UpdateBalancebyId;

                    ");                                                

                    connection.Execute(@"
                        CREATE PROCEDURE UpdateBalancebyId(IN Tid int)
                        BEGIN   
 
                             DECLARE Tdate DATE;
                            DECLARE accountId int;
    
                            SELECT t.date, t.accountId INTO Tdate, accountId
                            FROM Transactions t 
                            WHERE t.id = Tid;

                            IF Tdate IS NOT NULL AND accountId IS NOT NULL THEN
                                CALL UpdateBalancebyDateAccount(Tdate, accountId);
                            END IF;

                        END;
                    ");

                    connection.Execute(@"

                        DROP PROCEDURE IF EXISTS UpdateBalancebyId;

                    ");

                    connection.Execute(@"

                        DROP PROCEDURE IF EXISTS UpdateBalancebyDate;

                    ");

                    connection.Execute(@"

                        CREATE PROCEDURE TransactionPostSave(IN Tid int, IN TaccountId int, IN Tdate DATE)
                        BEGIN   		
	
	                        DECLARE TtranferAccount int;
	                        DECLARE TtranferDate DATE;	
	
 	                        START TRANSACTION;
 
 	                        select date,accountid into TtranferDate,TtranferAccount
 	                        from Transactions t 
 	                        where tranferid = Tid;
 
                            CALL UpdateTranfer(Tid);
   
   	                        if TtranferAccount is null then
   		                        select date,accountid into TtranferDate,TtranferAccount
	 	                        from Transactions t 
	 	                        where tranferid = Tid;
 	                        END IF;   	
   
	                        CALL UpdateBalancebyDateAccount(Tdate,TaccountId);
	                        CALL UpdateBalancebyDateAccount(TtranferDate,TtranferAccount);  

	                        COMMIT;

                        END;



                    ");

                    connection.Execute(@"

                        CREATE PROCEDURE SplitsPostSave(IN Sid int, IN Tid int)
                        BEGIN   		
	
	                        DECLARE TtranferAccount int;
	                        DECLARE TtranferDate DATE;	
	                        DECLARE TtransAccount int;
	                        DECLARE TtransDate DATE;	
	
 	                        START TRANSACTION;
 
 	                        select date,accountid into TtransDate,TtransAccount
 	                        from Transactions t 
 	                        where id = Tid;
 
 	                        select date,accountid into TtranferDate,TtranferAccount
 	                        from Transactions t 
 	                        where tranferSplitid = Sid;
 
                            CALL UpdateTransactionWithSplit(Tid);
                            CALL UpdateTranferSplit(Sid);
    
   	                        if TtranferAccount is null then
   		                        select date,accountid into TtranferDate,TtranferAccount
	 	                        from Transactions t 
	 	                        where tranferSplitid = Sid;
 	                        END IF;   	
   
	                        CALL UpdateBalancebyDateAccount(TtransDate,TtransAccount);
	                        CALL UpdateBalancebyDateAccount(TtranferDate,TtranferAccount);  

	                        COMMIT;

                        END;

                    ");

                    connection.Execute(@"
                        INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202405272326', '5.0');
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
