using Dapper;
using GARCA.wsData.Repositories;

namespace GARCA.wsData.Migrations.Seeder
{
    public static class PeriodsRemindersSeed
    {
        public static void Do()
        {
            dbContext.OpenConnection().Execute(@"
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
