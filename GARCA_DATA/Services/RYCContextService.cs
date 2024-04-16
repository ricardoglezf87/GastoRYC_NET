using MySqlConnector;

namespace GARCA.Data.Services
{
    public class RycContextService
    {
        public MySqlConnection getConnection()
        {
            MySqlConnection connection;

#if DEBUG
                connection = new MySqlConnection("Server=192.168.1.142;Database=GARCA_PRE;User Id=arcarw;Password=3DWw^3PUuW$B@E8B$Z");
#else
                connection = new MySqlConnection("Server=192.168.1.142;Database=GARCA;User Id=arcarw;Password=3DWw^3PUuW$B@E8B$Z");
#endif

            connection.Open();
            return connection;
        }
    }
}
