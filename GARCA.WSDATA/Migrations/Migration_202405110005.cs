using Dapper;
using GARCA.Utils.Logging;
using GARCA.wsData.Migrations.Seeder;
using GARCA.wsData.Repositories;


namespace wsData.Migrations
{
    public class Migration_202405110005
    {
        public  void Do()
        {
            try
            {
                using (var connection = DBContext.OpenConnection(true))
                {
                    connection.Execute(@"

                        CREATE TRIGGER Accounts_Categories_IT
                        BEFORE INSERT ON Accounts
                        FOR EACH ROW
                        BEGIN
                            DECLARE idCat INT;
                            INSERT INTO Categories (description,categoriesTypesid) VALUES (CONCAT('[', NEW.description, ']'),3);
                            SET idCat = LAST_INSERT_ID();
                            SET NEW.categoryid = idCat;
                        END;
 
                        ");

                        connection.Execute(@"

                        CREATE TRIGGER Accounts_Categories_UT
                        BEFORE UPDATE ON Accounts
                        FOR EACH ROW
                        BEGIN
                            UPDATE Categories SET description = CONCAT('[', NEW.description, ']') WHERE id = OLD.categoryid;
                        END;
                        ");

                        connection.Execute(@"

                        CREATE TRIGGER Accounts_Categories_DT
                        BEFORE DELETE ON Accounts
                        FOR EACH ROW
                        BEGIN
                            DELETE FROM Categories WHERE id = OLD.categoryid;
                        END;

 
                        ");

                        connection.Execute(@"

                        ALTER TABLE Accounts DROP FOREIGN KEY FK_Accounts_Categories_Categoryid;
                        
                        ");                        

                    connection.Execute(@"
                        INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202405110005', '5.0');
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
