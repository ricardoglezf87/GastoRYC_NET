using Dapper;
using GARCA.wsData.Repositories;

namespace GARCA.wsData.Migrations.Seeder
{
    public static class PeriodsRemindersSeed
    {
        public static void Do()
        {
            using (var connection = DBContext.OpenConnection())
            {
                connection.Execute(@"
                    INSERT INTO PeriodsReminders (description) VALUES
	                 ('Diario'),
	                 ('Semanal'),
	                 ('Mensual'),
	                 ('Bimestral'),
	                 ('Trimestral'),
	                 ('Semestral'),
	                 ('Anual');
                ");
            }
        }

    }
}
