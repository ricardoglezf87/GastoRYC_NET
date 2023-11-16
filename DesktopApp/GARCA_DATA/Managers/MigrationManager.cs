using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
                if (await IsMigrateFeature(clase.Name)) continue;

                object instancia = Activator.CreateInstance(clase);
                
                MethodInfo metodoDo = clase.GetMethod("Do");

                if (metodoDo != null)
                {
                    await Task.Run(() => metodoDo.Invoke(instancia, null));
                }
                else
                {
                    throw new Exception($"No se cuentra el metodo Do en la clase {clase.Name}");
                }
            }
        }

        public async Task<bool> IsMigrateFeature(string feature)
        {
            return Convert.ToInt32(await iRycContextService.getConnection().ExecuteScalarAsync($"Select count(*) from MigrationsHistory where MigrationId='{feature}'")) != 0;
        }
    }
}
