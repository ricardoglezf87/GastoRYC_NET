using Dapper;
using GARCA.wsData.Repositories;

namespace GARCA.wsData.Migrations.Seeder
{
    public static class CategoriesTypesSeed
    {
        public static void Do()
        {
            using (var connection = dbContext.OpenConnection())
            {
                connection.Execute(@"
                    INSERT INTO CategoriesTypes (description) VALUES
	                 ('Gastos'),
	                 ('Ingresos'),
	                 ('Transferencia'),
	                 ('Especiales');
                ");
            }
        }

    }
}
