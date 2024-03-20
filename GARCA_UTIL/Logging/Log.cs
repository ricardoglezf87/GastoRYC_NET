using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GARCA.Utils.Logging
{
    public static class Log
    {
        private static string logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"Logs");
        public static int logLevel = 1; // 0 = All; 1 = Only Errors

        private static string createFile()
        {
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            return Path.Combine(logPath, "error_log.txt");
        }

        public static void LogError(string errorMessage)
        {
            try
            {
                string filePath = createFile();                
                string logMessage = $"{DateTime.Now}: {errorMessage}";                
                File.AppendAllText(filePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al escribir en el archivo de registro: {ex.Message}");
            }
        }

        public static void LogInformation(string infoMessage)
        {
            if (logLevel == 1)
            {
                return;
            }

            try
            {
                string filePath = createFile();                
                string logMessage = $"{DateTime.Now}: {infoMessage}";                
                File.AppendAllText(filePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al escribir en el archivo de registro: {ex.Message}");
            }
        }

    }
}
