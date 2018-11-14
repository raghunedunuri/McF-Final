using System.Data;

namespace McF.DataAcess
{
    public interface IDbHelper
    {
        void AddParameter(IDbCommand dbCommand, string parameterName, object value);
        void AddParameter(string parameterName, object value);
       
        void BeginTransaction();
        
        void CloseConnection();
        
        void CommitTransaction();

        IDbCommand CreateCommand(string commandText);

        IDbCommand CreateCommand(string commandText, CommandType commandType);

        IDbCommand CreateCommand(IDbConnection connection, string commandText, CommandType commandType);
        DataSet ExecuteDataSet();

        int ExecuteNonQuery();

        IDataReader ExecuteReader();

        object ExecuteScalar();

        IDbConnection OpenConnection();

        void RollbackTransaction();
    }
}
