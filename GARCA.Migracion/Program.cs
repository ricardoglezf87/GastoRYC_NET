using GARCA.Utils.Logging;
using GARCA.wsData.Repositories;

Console.WriteLine("Proceso de Migración...");

try
{
#if !TEST
    throw new Exception("No se puede ejecutar con esta configuración");
#endif


    Console.WriteLine("¿Desea limpiar la base de datos?(Y/N):");
    
    string res = Console.ReadLine();
    
    if (res != null && res.ToLower() == "y")
    {
        Console.WriteLine("Se inicia el proceso de borrado....");
        
        MigrationRepository.CleanDataBase();
        
        Console.WriteLine("Borrado correctamente....");
    }
    
    Console.WriteLine("Se inicia el proceso de migración....");
    
    MigrationRepository.Migrate();
    
    Console.WriteLine("Migrado correctamente....");

    Console.ReadLine();
}
catch (Exception ex)
{
    Log.LogError(ex.Message);
    Console.WriteLine(ex.Message);
}