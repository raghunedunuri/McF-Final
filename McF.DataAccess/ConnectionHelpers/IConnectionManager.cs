using System.Data;

namespace McF.DataAcess
{
    public interface IConnectionManager
    {
        IDbConnection GetConnection();
    }
}
