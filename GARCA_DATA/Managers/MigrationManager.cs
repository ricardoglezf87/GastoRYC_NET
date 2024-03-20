using Dapper;
using GARCA_UTIL.Exceptions;
using System.Reflection;

using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class MigrationManager
    {
        public async Task Migrate()
        {
            Type[] clases = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Namespace == "GARCA.Data.Migrations" && t.IsClass && !t.IsNested)
                .ToArray();

            foreach (Type clase in clases)
            {
                if (await IsMigrateFeature(clase.Name))
                {
                    continue;
                }

                object? instancia = Activator.CreateInstance(clase);

                MethodInfo? metodoDo = clase.GetMethod("Do");

                if (metodoDo != null)
                {
                    await Task.Run(() => metodoDo.Invoke(instancia, null));
                }
                else
                {
                    throw new MigrationException($"No se cuentra el metodo Do en la clase {clase.Name}");
                }
            }
        }

        public async Task<bool> IsMigrateFeature(string feature)
        {
            using (var connection = iRycContextService.getConnection())
            {
                return Convert.ToInt32(await connection.ExecuteScalarAsync($"Select count(*) from MigrationsHistory where MigrationId='{feature}'")) != 0;
            }
        }
    }
}
