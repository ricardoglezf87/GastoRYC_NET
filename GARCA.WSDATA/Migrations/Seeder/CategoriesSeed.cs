using Dapper;
using GARCA.wsData.Repositories;

namespace GARCA.wsData.Migrations.Seeder
{
    public static class CategoriesSeed
    {
        public static void Do()
        {
            using (var connection = dbContext.OpenConnection())
            {
                connection.Execute(@"                    
                    INSERT INTO Categories VALUES 
                     (-1,'Splits...',4),
                     (0,'*** Sin Categoría ***',4);

                    UPDATE Categories SET id = 0 where id = 1;
                
                ");
            }
        }

    }
}
