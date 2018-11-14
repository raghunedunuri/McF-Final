using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace McF.DataAcess
{
    public class SqlConnectionManager : IConnectionManager
    {
        public IDbConnection GetConnection()
        {
            //string connectionString = "Data Source=mckeany.saven.in;UID=McF;password=saven1234;database=McF;Connection Timeout=120";
            //string connectionString = "Data Source=192.168.195.138;UID=McF;password=saven1234;database=McF;Connection Timeout=120";
            string connectionString = "Data Source=info.mckeany-flavell.com;UID=McKeany;password=McF@1234;database=McF;Connection Timeout=120";

            if ( ConfigurationManager.ConnectionStrings["McFConnectionString"] != null )
                connectionString = ConfigurationManager.ConnectionStrings["McFConnectionString"].ConnectionString;
            return new SqlConnection(connectionString);
        }
    }
}
