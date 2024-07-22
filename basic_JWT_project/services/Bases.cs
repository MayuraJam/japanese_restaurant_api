using Microsoft.Data.SqlClient;

namespace basic_JWT_project.services
{
    public abstract class Bases
    {
        public SqlConnection CreateSQLConnection()
        {
            return new SqlConnection(Environment.GetEnvironmentVariable("DBConnection"));
        }
    }
}
