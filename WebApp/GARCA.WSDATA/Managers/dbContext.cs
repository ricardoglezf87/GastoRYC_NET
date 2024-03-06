using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GARCA.wsData.Managers
{
    static class dbContext
    {
        //const string connectionString = "Server=192.168.1.142;Database=GARCA_PRE;User Id=arcadb;Password=5*GEs4*8q8WGy!f8KU;";
        const string connectionString = "Server=192.168.1.142;Database=GARCA_PRE;User Id=arcarw;Password=3DWw^3PUuW$B@E8B$Z";

        public static IDbConnection OpenConnection()
        {
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
