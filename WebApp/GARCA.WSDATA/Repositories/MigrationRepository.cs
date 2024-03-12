using Dapper;
using GARCA_UTIL.Exceptions;
using System.Reflection;



namespace GARCA.wsData.Repositories
{
    public static class MigrationRepository
    {
        public static void Migrate()
        {
            dbContext.OpenConnection(true).Execute(@"

                -- MigrationsHistory definition

               CREATE TABLE IF NOT EXISTS `MigrationsHistory` (
                  `MigrationId` text DEFAULT NULL,
                  `ProductVersion` varchar(100) DEFAULT NULL
                );
            ");


            Type[] clases = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Namespace == "wsData.Migrations" && t.IsClass && !t.IsNested)
                .ToArray();

            foreach (Type clase in clases)
            {
                if (IsMigrateFeature(clase.Name))
                {
                    continue;
                }

                object? instancia = Activator.CreateInstance(clase);

                MethodInfo? metodoDo = clase.GetMethod("Do");

                if (metodoDo != null)
                {
                     metodoDo.Invoke(instancia, null);
                }
                else
                {
                    throw new MigrationException($"No se cuentra el metodo Do en la clase {clase.Name}");
                }
            }
        }

        public static bool IsMigrateFeature(string feature)
        {
            return Convert.ToInt32(dbContext.OpenConnection().ExecuteScalar($"Select count(*) from MigrationsHistory where MigrationId='{feature}'")) != 0;
        }
    }
}
