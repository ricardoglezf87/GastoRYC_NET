using Dapper;
using GARCA.wsData.Repositories;

namespace GARCA.wsData.Migrations.Seeder
{
    public static class AccountsTypesSeed
    {
        public static void Do()
        {
            dbContext.OpenConnection().Execute(@"
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
