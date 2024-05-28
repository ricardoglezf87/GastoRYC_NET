using Dapper;
using GARCA.wsData.Repositories;

namespace GARCA.wsData.Migrations.Seeder
{
    public static class TransactionsStatusSeed
    {
        public static void Do()
        {
            using (var connection = DBContext.OpenConnection())
            {
                connection.Execute(@"
                    INSERT INTO TransactionsStatus (description) VALUES
	                 ('Pendiente'),
	                 ('Provisional'),
	                 ('Conciliado');
                ");
            }
        }

    }
}
