using Microsoft.Data.SqlClient;

namespace japanese_resturant_project.services
{
    public abstract class Bases
    {
        public SqlConnection CreateSQLConnection()
        {
            return new SqlConnection(Environment.GetEnvironmentVariable("DBConnection"));
        }
    }
}
