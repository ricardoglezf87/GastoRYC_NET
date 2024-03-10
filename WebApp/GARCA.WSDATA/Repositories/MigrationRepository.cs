using Dapper;
using GARCA_UTIL.Exceptions;
using System.Reflection;



namespace GARCA.wsData.Repositories
{
    public static class MigrationRepository
    {
        public static async Task Migrate()
        {
            Type[] clases = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Namespace == "wsData.Migrations" && t.IsClass && !t.IsNested)
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

        public static async Task<bool> IsMigrateFeature(string feature)
        {
            return Convert.ToInt32(await dbContext.OpenConnection().ExecuteScalarAsync($"Select count(*) from MigrationsHistory where MigrationId='{feature}'")) != 0;
        }
    }
}
