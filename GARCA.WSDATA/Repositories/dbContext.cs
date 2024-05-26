using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GARCA.wsData.Repositories
{
    public static class DBContext
    {
        public static IDbConnection OpenConnection(bool migration = false)
        {
            MySqlConnection connection;

#if DEBUG
            if (migration)
            {
                connection = new MySqlConnection("Server=192.168.1.142;Database=GARCA_PRE;User Id=arcadb;Password=5*GEs4*8q8WGy!f8KU;");
            }else
            {
                connection = new MySqlConnection("Server=192.168.1.142;Database=GARCA_PRE;User Id=arcarw;Password=3DWw^3PUuW$B@E8B$Z");
            }
#else
#if TEST

            if (migration)
            {
                connection = new MySqlConnection("Server=192.168.1.142;Database=GARCA_TEST;User Id=arcadb;Password=5*GEs4*8q8WGy!f8KU;");
            }else
            {
                connection = new MySqlConnection("Server=192.168.1.142;Database=GARCA_TEST;User Id=arcarw;Password=3DWw^3PUuW$B@E8B$Z");
            }
#else
            if (migration)
            {
                connection = new MySqlConnection("Server=192.168.1.142;Database=GARCA;User Id=arcadb;Password=5*GEs4*8q8WGy!f8KU;");
            }else
            {
                connection = new MySqlConnection("Server=192.168.1.142;Database=GARCA;User Id=arcarw;Password=3DWw^3PUuW$B@E8B$Z");
            }
#endif
#endif

            connection.Open();
            return connection;
        }
    }
}
