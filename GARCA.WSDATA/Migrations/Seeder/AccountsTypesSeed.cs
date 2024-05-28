using Dapper;
using GARCA.wsData.Repositories;

namespace GARCA.wsData.Migrations.Seeder
{
    public static class AccountsTypesSeed
    {
        public static void Do()
        {
            using (var connection = DBContext.OpenConnection())
            {
                connection.Execute(@"
                    INSERT INTO AccountsTypes (description) VALUES
	                    ('Efectivo'),
	                    ('Banco'),
	                    ('Tarjetas'),
	                    ('Inversiones'),
	                    ('Prestamos'),
	                    ('Inmovilizados'),
	                    ('Ahorros');
                ");
            }
        }

    }
}
