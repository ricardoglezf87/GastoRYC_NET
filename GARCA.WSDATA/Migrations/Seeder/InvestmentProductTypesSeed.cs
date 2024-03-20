using Dapper;
using GARCA.wsData.Repositories;

namespace GARCA.wsData.Migrations.Seeder
{
    public static class InvestmentProductTypesSeed
    {
        public static void Do()
        {
            using (var connection = dbContext.OpenConnection())
            {
                connection.Execute(@"
                    INSERT INTO InvestmentProductsTypes (description) VALUES
	                 ('Cryptomonedas'),
	                 ('Fondo de inversión'),
	                 ('Plan de pensiones'),
	                 ('Acciones'),
	                 ('Crowlending'),
	                 ('Cuenta Ahorro'),
	                 ('Deposito');
                ");
            }
        }

    }
}
