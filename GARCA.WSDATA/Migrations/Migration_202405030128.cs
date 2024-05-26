using Dapper;
using GARCA.Utils.Logging;
using GARCA.wsData.Migrations.Seeder;
using GARCA.wsData.Repositories;


namespace wsData.Migrations
{
    public class Migration_202405030128
    {
        public  void Do()
        {
            try
            {
                using (var connection = DBContext.OpenConnection(true))
                {
                    connection.Execute(@"
                        CREATE PROCEDURE UpdateTranfer(IN Tid INT)
                        BEGIN
   
                            START TRANSACTION;

   
                            -- Borra cuando no se selecciona cuenta de transferencia
                            delete tt 
                            from Transactions t
    	                        inner join Transactions tt on t.tranferid = tt.id 
    	                        inner join Categories c on t.categoryid = c.id     	
                            where t.id = Tid and c.categoriesTypesid != 3;    
   
   
   	                        UPDATE Transactions t
		                        LEFT JOIN Transactions tt ON t.tranferid = tt.id 
		                        INNER JOIN Categories c ON t.categoryid = c.id
	                        SET 
		                        t.tranferid = NULL
	                        WHERE t.id = Tid AND t.tranferid IS NOT NULL AND tt.id IS NULL AND c.categoriesTypesid != 3;
	    
                            -- Actualiza cuando existe transacción y se ha seleccionado cuenta de transferencia
    
                            UPDATE Transactions tt
	                        INNER JOIN Transactions t ON t.tranferid = tt.id
		                        inner join Accounts ac on t.accountid = ac.id 
	                        INNER JOIN Categories c ON t.categoryid = c.id
		                        inner join Accounts ca on ca.categoryid = c.id 
	                        SET 
		                        tt.date = t.date,
		                        tt.accountid = ca.id, 
		                        tt.personid = t.personid,
		                        tt.categoryid = ac.categoryid,
		                        tt.amountIn = t.amountOut,
		                        tt.amountOut = t.amountIn,
		                        tt.memo = t.memo,
		                        tt.tagid = t.tagid,
		                        tt.transactionStatusid = t.transactionStatusid 
	                        WHERE t.id = Tid AND c.categoriesTypesid = 3;

	                        -- Crea cuando no existe transacción y se ha seleccionado cuenta de transferencia
    
                            insert into Transactions (date,accountid,personid,categoryid,amountIn,amountOut,memo,tagid,transactionStatusid, tranferid)
	                        SELECT t.date,ca.id, t.personid,ac.categoryid, t.amountOut,t.amountIn,t.memo,t.tagid, t.transactionStatusid, t.id  
	                        from Transactions t
			                        inner join Accounts ac on t.accountid = ac.id 
		                        INNER JOIN Categories c ON t.categoryid = c.id
			                        inner join Accounts ca on ca.categoryid = c.id 
	                            left join Splits s on s.tranferid = t.id
	                        WHERE t.id = Tid AND c.categoriesTypesid = 3 and t.tranferid is null and s.id is null;		

	                        update Transactions t 
	                        inner join Transactions tt on t.id = tt.tranferid 
	                        set
		                        t.tranferid = tt.id 
	                        where t.id = Tid and t.tranferid is null;

                            -- Borra las transacciones cundo han sido borradas las transacciones en el otro lado
                            delete
                            from Transactions 
                            where tranferid = Tid and not exists(select * from Transactions t where t.id = Tid);
        
        
                            COMMIT;
                        END
                    ");

                    connection.Execute(@"
                        INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202405030128', '5.0');
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
