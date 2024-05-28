using Dapper;
using GARCA.Utils.Logging;
using GARCA.wsData.Migrations.Seeder;
using GARCA.wsData.Repositories;


namespace wsData.Migrations
{
    public class Migration_202405040158
    {
        public  void Do()
        {
            try
            {
                using (var connection = DBContext.OpenConnection(true))
                {
                    connection.Execute(@"
                        CREATE PROCEDURE UpdateTranferSplit(IN Sid INT)
                        BEGIN
      
                        -- Borra cuando no se selecciona cuenta de transferencia
                        delete tt 
                        from Splits t
    	                    inner join Transactions tt on t.tranferid = tt.id 
    	                    inner join Categories c on t.categoryid = c.id     	
                        where t.id = Sid and c.categoriesTypesid != 3;    
   
   
   	                    UPDATE Splits t
		                    LEFT JOIN Transactions tt ON t.tranferid  = tt.id 
		                    INNER JOIN Categories c ON t.categoryid = c.id
	                    SET 
		                    t.tranferid = NULL
	                    WHERE t.id = Sid AND t.tranferid IS NOT NULL AND tt.id IS NULL AND c.categoriesTypesid != 3;
	    
                        -- Actualiza cuando existe transacción y se ha seleccionado cuenta de transferencia
    
                        UPDATE Transactions tt
	                    INNER JOIN Splits s ON s.tranferid = tt.id
	                    inner join Transactions t on t.id = s.transactionid 
		                    inner join Accounts ac on t.accountid = ac.id 
	                    INNER JOIN Categories c ON s.categoryid = c.id
		                    inner join Accounts ca on ca.categoryid = c.id 
	                    SET 
		                    tt.date = t.date,
		                    tt.accountid = ca.id, 
		                    tt.personid = t.personid,
		                    tt.categoryid = ac.categoryid,
		                    tt.amountIn = s.amountOut,
		                    tt.amountOut = s.amountIn,
		                    tt.memo = s.memo,
		                    tt.tagid = s.tagid,
		                    tt.transactionStatusid = t.transactionStatusid 
	                    WHERE s.id = Sid AND c.categoriesTypesid = 3;

	                    -- Crea cuando no existe transacción y se ha seleccionado cuenta de transferencia
    
                        insert into Transactions (date,accountid,personid,categoryid,amountIn,amountOut,memo,tagid,transactionStatusid, tranferSplitid)
	                    SELECT t.date,ca.id, t.personid,ac.categoryid, s.amountOut,s.amountIn,s.memo,s.tagid, t.transactionStatusid, s.id  
	                    from Transactions t
			                    inner join Splits s on s.transactionid = t.id 
			                    inner join Accounts ac on t.accountid = ac.id 
		                    INNER JOIN Categories c ON s.categoryid = c.id
			                    inner join Accounts ca on ca.categoryid = c.id 
	                    WHERE s.id = Sid AND c.categoriesTypesid = 3 and s.tranferid is null;	

	                    update Splits  s 
		                    inner join Transactions t on t.tranferSplitid = s.id 
	                    set
		                    s.tranferid = t.id 
	                    where s.id = Sid and s.tranferid is null;

                        -- Borra las transacciones cundo han sido borradas las transacciones en el otro lado
                        delete
                        from Transactions 
                        where tranferSplitid = Sid and not exists(select * from Splits s where s.id = Sid);
        
                        END
                    ");

                    connection.Execute(@"
                        INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202405040158', '5.0');
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
